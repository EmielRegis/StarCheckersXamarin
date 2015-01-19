using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Net;
using StarCheckersWindows.Source.Figures;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.Remoting;


namespace StarCheckersWindows
{
    class StarNetGameplayScreen : GameplayScreen
    {
        private Point selectedFigurePrevPosition = new Point(-1, -1);
        private List<Point> destPoints = new List<Point>();
        private List<Point> rmFigsPositions = new List<Point>();
        private SyncClient client;
        private IList<Figure> remFigs = new List<Figure>();
        private IList<Point> destPts = new List<Point>();
        private bool isWhite;

        public StarNetGameplayScreen()
        {
            InitializeConnection();
        }

        public StarNetGameplayScreen(string whiteFiguresTemplate, string blackFiguresTemplate) : base(whiteFiguresTemplate, blackFiguresTemplate)
        {
            InitializeConnection();
        }
            
        private void InitializeConnection()
        {
            try
            {
//                client = new SyncClient(IPAddress.Parse("25.113.123.152"), 8888);
                            client = new SyncClient(IPAddress.Parse("192.168.0.11"), 8888);
                //            client = new SyncClient(IPAddress.Parse("25.122.152.24"), 8888);

                client.StartClient();




                string response;
                response = client.ReceiveMessage();
                if(response == "white")
                {
                    isWhite = true;
                    whiteFigures = playerFigures;
                    blackFigures = enemyFigures;
                    isPlayerTurn = true;
                    isEnemyTurn = false;
                }
                else if(response == "black")
                {
                    whiteFigures = enemyFigures;
                    blackFigures = playerFigures;
                    isWhite = false;
                    isPlayerTurn = false;
                    isEnemyTurn = true;
                }
                else if(response == "any")
                {
                    client.StopClient();
                }


                PlayerWins += () =>
                    {
                        if(client.serverSocket.Connected)
                        {
                            client.SendMessage("end");
                            client.StopClient();
                        }

                    };

                EnemyWins += () =>
                    {
                        if(client.serverSocket.Connected)
                        {
                            client.SendMessage("end");
                            client.StopClient();
                        }

                    };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        protected override void PreparePlayerFigure(Figure f)
        {
            f.MovingEnded += (x, y) =>
                {
                    if (f.YPosition == 0 && f.GetType() == typeof (Man))
                    {
                        playerFigures.Remove(f);
                        f = new King(f.XPosition, f.YPosition, f.FigureColor);
                        f.Image = (f.FigureColor == FigureColor.White) ? whiteKingImage : blackKingImage;
                        PreparePlayerFigure(f);
                        playerFigures.Add(f);
                    }

                    isPlayerTurn = false;
                    isEnemyTurn = true;
                    if(client.serverSocket.Connected)
                    {
                        string message = JsonConvert.SerializeObject(new NetworkMessage(NetworkMessageType.MOVE, 
                            new Move(selectedFigurePrevPosition, new Point(f.XPosition, f.YPosition))));
                            client.SendMessage(message);
                        selectedFigurePrevPosition.X = -1;
                    }
                };

            f.AttackingEnded += (fig, x, y) =>
                {
                    fig.IsAlive = false;
                    enemyFigures.Remove(fig);

                    if (f.GeneratePossibleAttacks(playerFigures, enemyFigures).Any())
                    {
                        isRecursiveCaptureInProgress = true;
                        f.IsSelected = true;
                        selectedFigure = f;
                        possibleAttacks = f.GeneratePossibleAttacks(playerFigures, enemyFigures, isRecursive: true);

                        destPoints.Add(new Point(f.XPosition, f.YPosition));
                        rmFigsPositions.Add(new Point(fig.XPosition, fig.YPosition));
                    }
                    else
                    {
                        if (f.YPosition == 0 && f.GetType() == typeof (Man))
                        {
                            playerFigures.Remove(f);
                            f = new King(f.XPosition, f.YPosition, f.FigureColor);
                            f.Image = (f.FigureColor == FigureColor.White) ? whiteKingImage : blackKingImage;
                            PreparePlayerFigure(f);
                            playerFigures.Add(f);
                        }
                        isRecursiveCaptureInProgress = false;
                        isPlayerTurn = false;
                        isEnemyTurn = true;

                        destPoints.Add(new Point(f.XPosition, f.YPosition));
                        rmFigsPositions.Add(new Point(fig.XPosition, fig.YPosition));

                        if(client.serverSocket.Connected)
                        {
                            string message = JsonConvert.SerializeObject(new NetworkMessage(NetworkMessageType.ATTACK, 
                                new Attack {
                                ActPos = selectedFigurePrevPosition,
                                DestPos = destPoints,
                                RemFigsPos = rmFigsPositions
                            }));
                            client.SendMessage(message);
                            selectedFigurePrevPosition.X = -1;
                            rmFigsPositions.Clear();
                            destPoints.Clear();
                        }
                    }
                };
        }

        protected override void PrepareEnemyFigure(Figure f)
        {
            f.MovingEnded += (x, y) =>
                {
                    if (f.YPosition == 7 && f.GetType() == typeof(Man))
                    {
                        enemyFigures.Remove(f);
                        f = new King(f.XPosition, f.YPosition, f.FigureColor);
                        f.Image = (f.FigureColor == FigureColor.White) ? whiteKingImage : blackKingImage;
                        PrepareEnemyFigure(f);
                        enemyFigures.Add(f);
                    }
                    f.IsSelected = false;
                    isPlayerTurn = true;
                    isEnemyTurn = false;
                };

            f.AttackingEnded += (fig, x, y) =>
                {
                    fig.IsAlive = false;
                    playerFigures.RemoveAll(rf => rf.XPosition == fig.XPosition && rf.YPosition == fig.YPosition);
                    remFigs.RemoveAt(0);
                    destPts.RemoveAt(0);

                    if (remFigs.Any())
                    {
                        isRecursiveCaptureInProgress = true;
                        f.IsSelected = true;
                        selectedFigure = f;
                        selectedFigure.Attack(remFigs.First(), destPts.First().X, destPts.First().Y);
                        selectedFigure.IsSelected = false;
                        selectedFigure = null;
                        isEnemyTurn = false;
                    }
                    else
                    {
                        if (f.YPosition == 7 && f.GetType() == typeof(Man))
                        {
                            enemyFigures.Remove(f);
                            f = new King(f.XPosition, f.YPosition, f.FigureColor);
                            f.Image = (f.FigureColor == FigureColor.White) ? whiteKingImage : blackKingImage;
                            PrepareEnemyFigure(f);
                            enemyFigures.Add(f);
                        }
                        f.IsSelected = false;
                        isRecursiveCaptureInProgress = false;
                        isPlayerTurn = true;
                        isEnemyTurn = false;
                    }
                };
        }

        public override void LoadContent()
        {
            base.LoadContent();

            if(client.serverSocket != null && client.serverSocket.Connected)
            {
                ScreenManager.Instance.AddOnExitApplicationAction(() => 
                    {if(client.serverSocket.Connected) client.SendMessage("user_disconnected");});
            }
            else
            {
                ScreenManager.Instance.ChangeScreens("TitleScreen");
            }

        }


        protected override void InsideUpdateAction()
        {
            if (isEnemyTurn && client.serverSocket.Connected)
            {
                string answer = client.ReceiveMessage().Trim();
                if(answer != "end" && answer != "user_disconnected")
                {
                    NetworkMessage msg = JsonConvert.DeserializeObject<NetworkMessage>(answer);
                    if(msg.Type == NetworkMessageType.MOVE)
                    {
                        Move move = JsonConvert.DeserializeObject<Move>(msg.Obj.ToString());
                        selectedFigure = enemyFigures.First(f => f.XPosition == 7 - move.ActPos.X && f.YPosition == 7 - move.ActPos.Y);

                        selectedFigure.Move(7 - move.DestPos.X, 7 - move.DestPos.Y);
                        selectedFigure.IsSelected = false;
                        selectedFigure = null;
                        isEnemyTurn = false;

                    }
                    else if(msg.Type == NetworkMessageType.ATTACK)
                    {
                        Attack attack = JsonConvert.DeserializeObject<Attack>(msg.Obj.ToString());
                        selectedFigure = enemyFigures.First(f => f.XPosition == 7 - attack.ActPos.X && f.YPosition == 7 - attack.ActPos.Y);
                        remFigs = new List<Figure>();
                        destPoints = new List<Point>();
                        attack.RemFigsPos.ForEach(rfp => remFigs.Add(playerFigures.First(pf => pf.XPosition == 7 - rfp.X && pf.YPosition == 7 - rfp.Y)));
                        attack.DestPos.ForEach(dp => destPts.Add(new Point(7 - dp.X, 7 - dp.Y)));

                        if(remFigs.Any())
                        {
                            selectedFigure.Attack(remFigs.First(), destPts.First().X, destPts.First().Y);
                            selectedFigure.IsSelected = false;
                            selectedFigure = null;
                            isEnemyTurn = false;
                        }

                    }
                    else if (msg.Type == NetworkMessageType.MULTIPLE_ATTACK)
                    {

                    }
                    else
                    {

                    }
                }
                else if (answer == "end")
                {
                    client.StopClient();
                    ScreenManager.Instance.ChangeScreens("TitleScreen");
                }
                else if(answer == "user_disconnected")
                {
                    if(client.serverSocket.Connected)
                    {
                        client.SendMessage("end");
                        client.StopClient();
                    }
                    ScreenManager.Instance.ChangeScreens("TitleScreen");
                }

//
//                Figure f;
//                var ai = new BasicAI();
//                int result = ai.AlphaBetaMinMaxAlgorithm(0, 5, int.MinValue, int.MaxValue, true, enemyFigures, playerFigures, ai, out f,
//                    out remFigs, out destPts);
//
//                selectedFigure = f;
//
//                if (remFigs != null && remFigs.Any())
//                {
//                    selectedFigure.Attack(remFigs.First(), destPts.First().X, destPts.First().Y);
//                    selectedFigure.IsSelected = false;
//                    selectedFigure = null;
//                    isEnemyTurn = false;
//                }
//                else
//                {
//
//                }
            }
        }

        protected override void ChessboardClickAction()
        {

            if (isPlayerTurn)
            {              
                float dim = (Math.Min(ScreenManager.Instance.Dimensions.X, ScreenManager.Instance.Dimensions.Y))/8.0f;

                IList<Figure> activeFigures = playerFigures;
                IList<Figure> nonActiveFigures = enemyFigures;

                if (selectedFigure != null)
                {
                    if (possibleAttacks.Any(pt =>
                        pt.Item1.X*dim < InputManager.Instance.MouseOrTouchX && pt.Item1.Y*dim < InputManager.Instance.MouseOrTouchY &&
                        pt.Item1.X*dim + dim > InputManager.Instance.MouseOrTouchX && pt.Item1.Y*dim + dim > InputManager.Instance.MouseOrTouchY))
                    {
                        Tuple<Point, Figure> dest = possibleAttacks.First(p =>
                            p.Item1.X*dim < InputManager.Instance.MouseOrTouchX && p.Item1.Y*dim < InputManager.Instance.MouseOrTouchY &&
                            p.Item1.X*dim + dim > InputManager.Instance.MouseOrTouchX && p.Item1.Y*dim + dim > InputManager.Instance.MouseOrTouchY);

                        if(selectedFigurePrevPosition.X == -1) selectedFigurePrevPosition = new Point(selectedFigure.XPosition, selectedFigure.YPosition);
                        selectedFigure.Attack(dest.Item2, dest.Item1.X, dest.Item1.Y);
                        selectedFigure.IsSelected = false;
                        selectedFigure = null;
                    }
                    else if (possibleMoves.Any(pt =>
                        pt.X*dim < InputManager.Instance.MouseOrTouchX && pt.Y*dim < InputManager.Instance.MouseOrTouchY &&
                        pt.X*dim + dim > InputManager.Instance.MouseOrTouchX && pt.Y*dim + dim > InputManager.Instance.MouseOrTouchY) &&
                        !activeFigures.Any(
                            af =>
                            af.GeneratePossibleAttacks(activeFigures, nonActiveFigures,
                                (enemyFigures == activeFigures)).Any()))
                    {
                        Point dest = possibleMoves.First(p =>
                            p.X*dim < InputManager.Instance.MouseOrTouchX && p.Y*dim < InputManager.Instance.MouseOrTouchY &&
                            p.X*dim + dim > InputManager.Instance.MouseOrTouchX && p.Y*dim + dim > InputManager.Instance.MouseOrTouchY);

                        selectedFigurePrevPosition = new Point(selectedFigure.XPosition, selectedFigure.YPosition);
                        selectedFigure.Move(dest.X, dest.Y);
                        selectedFigure.IsSelected = false;
                        selectedFigure = null;
                    }
                }

                if (!isRecursiveCaptureInProgress)
                {
                    selectedFigure = playerFigures.FirstOrDefault(f => f.IsSelected);
                    if (selectedFigure != null)
                    {
                        possibleMoves = selectedFigure.GeneratePossibleMoves(playerFigures, enemyFigures);
                        possibleAttacks = selectedFigure.GeneratePossibleAttacks(playerFigures, enemyFigures);
                    }    
                }
            }
        }
    }
}
