using AirlineWeb.Dtos;

namespace AirlineWeb.MessageBus
{
    public interface IMessageBus
    {
        void SendMessage(NotificationMessageDto notificationMessageDto);
    }
}