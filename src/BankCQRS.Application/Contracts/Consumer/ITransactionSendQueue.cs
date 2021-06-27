using System;
using System.Collections.Generic;
using System.Text;

namespace BankCQRS.Application.Contracts.Consumer
{
    public interface ITransactionSendQueue
    {
        void SendQueue(string message);
    }
}
