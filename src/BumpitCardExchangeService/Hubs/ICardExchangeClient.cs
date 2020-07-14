﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace BumpitCardExchangeService
{
  public interface ICardExchangeClient
  {
    Task Subscribed(IEnumerable<string> peers);

    Task Unsubscribed(string statusMessage);

    Task GeolocationChanged(IEnumerable<string> peers);

    Task DisplayNameChanged(string statusMessage);

    Task CardExchangeRequested(string deviceId, string displayName);

    Task WaitingForAcceptance(string peerDeviceId);

    Task CardExchangeAccepted(string peerDeviceId);

    Task CardDataReceived(string deviceId, string displayName, string cardData);

    Task CardDataSent(string peerDeviceId);
  }
}