using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using PHDFinalVersion.ChatHub;
using PHDFinalVersion.Controllers;
using PHDFinalVersion.Models;

namespace PHDFinalVersion.ChatHubb
{
    public class ChatHubCommunication:Hub
    {
        private readonly SendAndReceiveMessage sendAndReceiveMessage1;
        public ChatHubCommunication(SendAndReceiveMessage sendAndReceiveMessage)
        {
            sendAndReceiveMessage1=sendAndReceiveMessage;
        }
        public async Task SendMessage(SendMessages formData)
        {
            SenderReceiverMessage ResponsefromController = sendAndReceiveMessage1.SendMessage1(formData);

            var responseMessage=JsonConvert.SerializeObject(ResponsefromController);

            await Clients.User(ResponsefromController.ReceiverId.ToString()).SendAsync("ReceiveMessage", responseMessage);
        }

        
    }
}
