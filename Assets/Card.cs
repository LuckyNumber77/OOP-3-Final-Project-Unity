using System;

public class Card
{
	public string Suit { get; set; }
	public string Value { get; set; }

	// Default constructor
	public Card(string suit, string value)
	{
		Suit = suit;
		Value = value;
	}

	// Optionally, you can add a method to display the card info
	public void DisplayCard()
	{
		Console.WriteLine($"Card: {Value} of {Suit}");
	}
}
