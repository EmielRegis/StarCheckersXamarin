namespace StarCheckersWindows
{
    public interface IFigureEvaluator
    {
        int EvaluateFigure(Figure figure, bool isReversed = false);
    }
}