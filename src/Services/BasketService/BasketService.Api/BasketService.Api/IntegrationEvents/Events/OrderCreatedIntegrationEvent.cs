﻿using BasketService.Api.Core.Domain;
using EventBus.Base.Events;

namespace BasketService.Api.IntegrationEvents.Events
{
    public class OrderCreatedIntegrationEvent : IntegrationEvent
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int OrderNumber { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
        public string State { get; set; }
        public string CardNumber { get; set; }
        public string CardHolderName { get; set; }
        public DateTime CardExpiration { get; set; }
        public string CardSecurityNumber { get; set; }
        public int CardTypeId { get; set; }
        public string Buyer { get; set; }
        public Guid RequestId { get; set; }
        public CustomerBasket Basket { get;}



        public OrderCreatedIntegrationEvent(string userId, string userName, string buyer, string city, string street, string state, string country, string zipCode, string cardNumber, string cardHolderName, string cardSecurityNumber, int cardTypeId, DateTime cardExpiration, CustomerBasket basket)
        {
            UserId = userId;
            UserName = userName;
            Buyer = buyer;
            City = city;
            Street = street;
            State = state;
            Country = country;
            ZipCode = zipCode;
            CardNumber = cardNumber;
            CardHolderName = cardHolderName;
            CardSecurityNumber = cardSecurityNumber;
            CardTypeId = cardTypeId;
            CardExpiration = cardExpiration;
            Basket = basket;
        }
    }
}
