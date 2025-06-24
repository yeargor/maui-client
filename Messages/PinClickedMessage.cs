namespace MauiDemo2.Messages
{
  public class PinClickedMessage
    {
        public string PinName { get; }
        public Location PinLocation { get; }

        public PinClickedMessage(string pinName, Location pinLocation)
        {
            PinName = pinName;
            PinLocation = pinLocation;
        }
    }
}