namespace FluentWpfChromes
{
    public interface IDelayed
    {
        int DragDelay { set; get; }
        int ResizeDelay { set; get; }
        bool SuppressLagging { set; get; }
    }
}
