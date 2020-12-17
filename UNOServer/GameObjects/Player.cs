///////////////////////////////////////////////////////////
//  Player.cs
//  Implementation of the Class Player
//  Generated by Enterprise Architect
//  Created on:      16-���-2020 9:59:03
//  Original author: adm-sabitovka
///////////////////////////////////////////////////////////


using System;
using System.Collections.Generic;
using System.Linq;
using UNOServer.ServerObjects;

namespace UNOServer.GameObjects {

	/// <summary>
	/// �����
	/// </summary>
	public class Player {

		public List<Card> Cards { get; set; }
		public string Name { get;  set; }
		public int Position { get; set; }
		public string Id { get; set; }

		private ServerObject server;

		/// 
		/// <param name="name"></param>
		public Player(string name) {
			Cards = new List<Card>();
			Name = name;
		}

		public string ShowCards() {
			return $"���� �����: {string.Join(" ", Cards.Select(� => �.DisplayValue))}";
		}

		/// 
		/// <param name="drawPile"></param>
		/// <param name="currentTurn"></param>
		public PlayerTurn PlayTurn(CardDeck drawPile, PlayerTurn previousTurn, ServerObject server) {
			this.server = server;
			string message = server.GetMessageFromPlayer("��� ���.\n:" + ShowCards(), this);

			short index = -1;
			Console.WriteLine("Player: " + message);
			while(!Int16.TryParse(message, out index)) {
				message = server.GetMessageFromPlayer(
					"�������� ������ ����� �����", this);
			}

			PlayerTurn turn = new PlayerTurn();
			if (previousTurn.Result == TurnResult.Skip
				|| previousTurn.Result == TurnResult.DrawTwo
				|| previousTurn.Result == TurnResult.WildDrawFour) {
				return ProcessAttack(previousTurn.Card, drawPile);

			} else if ((previousTurn.Result == TurnResult.WildCard
						  || previousTurn.Result == TurnResult.Attacked
						  || previousTurn.Result == TurnResult.ForceDraw)
						  && HasMatch(previousTurn.DeclaredColor)) {
				turn = PlayMatchingCard(previousTurn.DeclaredColor);
			}

			return turn;
			;
		}

        private PlayerTurn ProcessAttack(Card currentDiscard, CardDeck cardDeck) {
			PlayerTurn turn = new PlayerTurn();
			turn.Result = TurnResult.Attacked;
			turn.Card = currentDiscard;
			turn.DeclaredColor = currentDiscard.Color;

			if (currentDiscard.Value == CardValue.Skip) {
				Console.WriteLine("����� " + Name + " ���������� ���");
				server.BroadcastMessage("����� " + Name + " ���������� ���", Id);
				server.TargetMessage("�� ����������� ���", this);
				return turn;

			} else if (currentDiscard.Value == CardValue.DrawTwo) {
				Console.WriteLine("����� " + Name + " ����� ��� ����� � ���������� ���");
				server.BroadcastMessage("����� " + Name + " ����� ��� ����� � ���������� ���", Id);
				server.TargetMessage("�� ������ ��� ����� � ����������� ���!", this);
				Cards.AddRange(cardDeck.Draw(2));

			} else if (currentDiscard.Value == CardValue.DrawFour) {
				Console.WriteLine("����� " + Name + " ������ ����� ������ ����� � ���������� ���");
				server.BroadcastMessage("����� " + Name + " ������ ����� ������ ����� � ���������� ���");
				server.TargetMessage("�� ������ ������ ����� � ����������� ���!", this);
				Cards.AddRange(cardDeck.Draw(4));

			}

			return turn;
		}

		private short RequestCardNumber() {
			string message = server.GetMessageFromPlayer("�������� �����:\n" + ShowCards(), this);

			short index = -1;
			Console.WriteLine("Player: " + message);
			while (!Int16.TryParse(message, out index) && index < Cards.Count) {
				message = server.GetMessageFromPlayer(
					"�������� ������ ����� �����", this);
			}

			return index;
        }

		private short RequestCardColor() {
			short index = -1;
			string message;
			do {
				message = server.GetMessageFromPlayer(
								"�������� ���� �����:\n1: �������\n2: �������\n3: �����\n4: ������", this);

				Console.WriteLine($"{Name}: " + message);
			} while (!Int16.TryParse(message, out index) && !Enum.IsDefined(typeof(CardColor), index));

			return index;
		}

		private PlayerTurn PlayMatchingCard(CardColor color) {
			var turn = new PlayerTurn();
			turn.Result = TurnResult.PlayedCard;
			var matching = Cards.Where(x => x.Color == color || x.Color == CardColor.Wild).ToList();

			//���� �������� ������ ����� �����
			if (matching.All(x => x.Value == CardValue.DrawFour)) {
				// ������ ������ ���������
				// TODO: ������� ����������� ������ � ���, ��� � ���� �������� ������ ����� �����
				turn.Card = matching.First();
				// ����������� � ������ ���� �����, ������� �� �����
				turn.DeclaredColor = (CardColor) RequestCardColor();
				// ��������� ����������� ����� ����� �� ���������� ������
				turn.Result = TurnResult.WildCard;
				// ������� ��� ����� �� ������ ���� ������
				Cards.Remove(matching.First());

				return turn;
			}

			// ����������� ����� ����� � ������
			Card turnCard = Cards[RequestCardNumber()];
			// �������, ��� �� ������� �����, ������� ����� ��������
			while (!matching.Contains(turnCard)) {
				turnCard = Cards[RequestCardNumber()];
            }

			turn.Card = turnCard;
			turn.DeclaredColor = turnCard.Color;

			return turn;
        }

		private bool HasMatch(CardColor color) {
			return Cards.Any(x => x.Color == color || x.Color == CardColor.Wild);
		}

		private bool HasMatch(Card card) {
			return Cards.Any(x => x.Color == card.Color || x.Value == card.Value || x.Color == CardColor.Wild);
		}

	}//end Player

}