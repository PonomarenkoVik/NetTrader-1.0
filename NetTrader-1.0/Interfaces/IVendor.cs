using Interfaces.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IVendor
    {
        string VendorName { get; }
        bool CreateAccount(string login, string id, string pass);
        bool RemoveAccount(string id);

        Dictionary<string, IInstrument> GetAllInstruments();
        IInstrument GetInstrumentByName(string id);
        Task<IResult<T>> SendMessageToVendor<T>(Message mess);
        Quote3Message GetLevel2(IInstrument instrument);
        Task<Quote3Message> GetLevel2FromServer(IInstrument instrument);
        Task<IResult<List<IAsset>>> GetAssetsAsync(IAccount account);
        Task<IResult<List<IOrder>>> GetOrdersByAccount(IAccount account);
        IOrder GetOrderById(IInstrument instr, string id);
        void Subscribe(IInstrument instr);
        void UnSubscribe(IInstrument instr);
        event Action<Quote3Message> OnNewQuoteEvent;
    }
}
