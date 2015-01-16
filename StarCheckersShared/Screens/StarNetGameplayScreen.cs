using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarCheckersWindows
{
    class StarNetGameplayScreen : GameplayScreen
    {
        public StarNetGameplayScreen()
        {
            
        }

        public StarNetGameplayScreen(string whiteFiguresTemplate, string blackFiguresTemplate) : base(whiteFiguresTemplate, blackFiguresTemplate)
        {
            
        }

        protected override void PreparePlayerFigure(Figure f)
        {
            throw new NotImplementedException();
        }

        protected override void PrepareEnemyFigure(Figure f)
        {
            throw new NotImplementedException();
        }

        protected override void ChessboardClickAction()
        {
            throw new NotImplementedException();
        }
    }
}
