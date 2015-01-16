using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StarCheckersWindows.Source.Figures;

namespace StarCheckersWindows
{
    public abstract class Figure
    {
        public FigureColor FigureColor { get; set; }

        public int XPosition
        {
            get { return _xPosition; }

            set { _xPosition = (value >= 0.0f && value <= 7.0f) ? value : _xPosition; }
        }

        public int YPosition
        {
            get { return _yPosition; }

            set { _yPosition = (value >= 0 && value <= 7) ? value : _yPosition; }
        }

        public int XDestinationPosition
        {
            get { return _xDestinationPosition; }

            set { _xDestinationPosition = (value >= 0.0f && value <= 7.0f) ? value : _xDestinationPosition; }
        }

        public int YDestinationPosition
        {
            get { return _yDestinationPosition; }

            set { _yDestinationPosition = (value >= 0 && value <= 7) ? value : _yDestinationPosition; }
        }

        public Vector2 DrawingPosition;

        public Image Image { get; set; }

        public bool IsMoving { get; private set; }

        public bool IsAttacking { get; private set; }

        public bool IsSelected
        {
            get
            {
				if (InputManager.Instance.MouseLeftButtonPressed() || InputManager.Instance.IsTouch)
                {
                    float dim = (Math.Min(ScreenManager.Instance.Dimensions.X, ScreenManager.Instance.Dimensions.Y)) / 8.0f;
					_isSelected = (DrawingPosition.X * dim < InputManager.Instance.MouseOrTouchX &&
						DrawingPosition.Y * dim < InputManager.Instance.MouseOrTouchY &&
						DrawingPosition.X * dim + dim > InputManager.Instance.MouseOrTouchX &&
						DrawingPosition.Y * dim + dim > InputManager.Instance.MouseOrTouchY);
                }

                if (_isSelected)
                    if (FigureSelected != null)
                        FigureSelected();
                    else if (FigureDeselected != null)
                        FigureDeselected();

                return _isSelected;
            }
            set
            {
                _isSelected = value;

                if (_isSelected)
                    if (FigureSelected != null)
                        FigureSelected();
                    else if (FigureDeselected != null)
                        FigureDeselected();
            }
        }

        public bool IsAlive
        {
            get { return _isAlive; }
            set
            {
                _isAlive = value;
                if (!value && Dying != null) Dying();
            }
        }

        public event Action<int, int> MovingEnded;
        public event Action<Figure, int, int> AttackingEnded;
        public event Action Dying;
        public event Action FigureSelected;
        public event Action FigureDeselected;

        private int _xPosition, _xDestinationPosition;
        private int _yPosition, _yDestinationPosition;

        private bool _isSelected;
        private bool _isAlive;

        private Figure _attackedFigure;

        protected Figure()
        {
            _xPosition = _yPosition = 0;
            DrawingPosition = Vector2.Zero;

            FigureColor = FigureColor.Void;

            IsSelected = false;
            IsMoving = false;
            IsAlive = true;
        }

        protected Figure(int xPosition, int yPosition)
            : this()
        {
            XPosition = xPosition;
            YPosition = yPosition;
            DrawingPosition.X = XPosition;
            DrawingPosition.Y = YPosition;
        }

        protected Figure(int xPosition, int yPosition, FigureColor figureColor)
            : this(xPosition, yPosition)
        {
            FigureColor = figureColor;
        }

        public void LoadContent()
        {
			if (Image != null)
			{
				Image.LoadContent();
				Image.IsCorrectionWhileScalingNeeded = true;
			}               
        }

        public void UnloadContent()
        {
            if (Image != null)
                Image.UnloadContent();
        }

        public abstract IEnumerable<Point> GeneratePossibleMoves(IList<Figure> playerFigures, IList<Figure> enemyFigures, bool isReversed = false);
        public abstract IEnumerable<Tuple<Point, Figure>> GeneratePossibleAttacks(IList<Figure> playerFigures, IList<Figure> enemyFigures, bool isReversed = false, bool isRecursive = false);
        public abstract Figure Copy();
        public abstract int Value();

        public void Move(int xPosition, int yPosition)
        {
            XDestinationPosition = xPosition;
            YDestinationPosition = yPosition;

            IsMoving = true;
        }

        public void Attack(Figure attackedFigure, int xPositon, int yPosition)
        {
            IsAttacking = true;
            _attackedFigure = attackedFigure;
            Move(xPositon, yPosition);
        }

        public void Update(GameTime gameTime)
        {
			if (Image != null)
				Image.IsCorrectionWhileScalingNeeded = true;

            if (IsMoving)
            {
                if (Math.Abs(DrawingPosition.X - _xDestinationPosition) > 0.05f && Math.Abs(DrawingPosition.Y - _yDestinationPosition) > 0.05f)
                {
                    DrawingPosition.X += (DrawingPosition.X < _xDestinationPosition) ? 0.05f : -0.05f;
                    DrawingPosition.Y += (DrawingPosition.Y < _yDestinationPosition) ? 0.05f : -0.05f;
                }
                else
                {                    
                    DrawingPosition.X = _xPosition = _xDestinationPosition;
                    DrawingPosition.Y = _yPosition = _yDestinationPosition;

                    IsMoving = false;

                    if (IsAttacking)
                    {
                        IsAttacking = false;
                        _attackedFigure.IsAlive = false;

                        if (AttackingEnded != null)
                            AttackingEnded(_attackedFigure, XPosition, YPosition);

//                        _attackedFigure = null;
                    }
                    else
                    {
                        if (MovingEnded != null)
                            MovingEnded(XPosition, YPosition);
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Image image)
        {
            if (image == null)
                return;

            float dim = (Math.Min(ScreenManager.Instance.Dimensions.X, ScreenManager.Instance.Dimensions.Y)) / 8.0f;

            image.Position.X = DrawingPosition.X * dim;
            image.Position.Y = DrawingPosition.Y * dim;
			image.Scale.X = dim / image.SourceRectange.Width;
			image.Scale.Y = dim  / image.SourceRectange.Height ;

            image.Draw(spriteBatch);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Draw(spriteBatch, Image);
        }
    }
}
