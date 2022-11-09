using Domain.Enums.SmartEnum;

var creditCardStandard = CreditCard.FromName("Standard");
var creditCardPremium = CreditCard.FromName("Premium");
var creditCardPlatinum = CreditCard.FromName("Platinum");

Console.WriteLine($"Discount for {creditCardStandard} is {creditCardStandard.Discount:P}");
Console.WriteLine($"Discount for {creditCardPremium} is {creditCardPremium.Discount:P}");
Console.WriteLine($"Discount for {creditCardPlatinum} is {creditCardPlatinum.Discount:P}");

Console.ReadKey();
