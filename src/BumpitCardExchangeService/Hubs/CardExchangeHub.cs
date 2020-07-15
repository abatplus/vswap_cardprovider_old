﻿using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using BumpitCardExchangeService.Redis;

namespace BumpitCardExchangeService
{
  public class CardExchangeHub : Hub<ICardExchangeClient>, ICardExchangeHub
  {
    private readonly ISubscriptionDataRepository _repository;

    public CardExchangeHub(ISubscriptionDataRepository repository)
    {
      _repository = repository;
    }

    public async Task Subscribe(string deviceId, double longitude, double latitude, string displayName)
    {
      _repository.SaveSubscriber(deviceId, longitude, latitude, displayName);

      await Clients.Caller.Subscribed(_repository.GetNearestSubscribers(deviceId));
    }

    public async Task Unsubcribe(string deviceId)
    {
      _repository.DeleteSubscriber(deviceId);

      await Clients.Caller.Unsubscribed("Erfolgreich abgemeldet.");
    }

    public async Task Update(string deviceId, double longitude, double latitude, string displayName)
    {
      _repository.UpdateGeolocation(deviceId, longitude, latitude);
      _repository.UpdateSubcriberDescription(deviceId, displayName);

      await Clients.Caller.Updated(_repository.GetNearestSubscribers(deviceId));
    }

    public async Task RequestCardExchange(string deviceId, string peerDeviceId, string displayName)
    {
      //TODO: send to deviceIdOfCardOnwer
      await Clients.Client(peerDeviceId).CardExchangeRequested(deviceId, displayName);

      //TODO: send to deviceIdCaller request confirmation
      await Clients.Caller.WaitingForAcceptance(peerDeviceId, displayName);
    }

    public async Task AcceptCardExchange(string deviceId, string peerDeviceId, string displayName)
    {
      await Clients.Client(deviceId).CardExchangeAccepted(peerDeviceId, displayName);

      await Clients.Caller.AcceptanceSent(deviceId, displayName);
    }

    public async Task SendCardData(string deviceId, string peerDeviceId, string displayName, string cardData)
    {
      //TODO: check that deviceIdRecipient was published data to deviceIdCaller
      await Clients.Client(peerDeviceId).CardDataReceived(deviceId, displayName, cardData);

      await Clients.Caller.CardDataSent(peerDeviceId, displayName);
    }
  }
}