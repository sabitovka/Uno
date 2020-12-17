///////////////////////////////////////////////////////////
//  Card.cs
//  Implementation of the Class Card
//  Generated by Enterprise Architect
//  Created on:      16-���-2020 9:58:29
//  Original author: adm-sabitovka
///////////////////////////////////////////////////////////

namespace UNOServer.GameObjects {

	/// <summary>
	/// ����� �����
	/// </summary>
	public class Card {

		public CardColor Color { get; set; }
		public CardValue Value { get; set; }

		public string DisplayValue {
			get {
				if (Value == CardValue.Wild) {
					return Value.ToString();
                }
				return $"{Color.ToString()} {Value.ToString()}";
            }
        }

	}//end Card

}