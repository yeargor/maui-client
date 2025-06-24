namespace MauiDemo2.Messages
{
    public class PopupAnimationMessage
    {
        public bool IsVisible { get; }

        public PopupAnimationMessage(bool isVisible)
        {
            IsVisible = isVisible;
        }
    }
}