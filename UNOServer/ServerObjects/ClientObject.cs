﻿using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UNOServer.GameObjects;

namespace UNOServer.ServerObjects {
    public class ClientObject {

        protected internal string Id { get; private set; }
        protected internal NetworkStream Stream { get; private set; }
        protected internal string UserName { get; set; }

        private TcpClient client;
        private ServerObject server;

        public ClientObject(TcpClient tcpClient) {
            Id = Guid.NewGuid().ToString();
            client = tcpClient;
        }

        public ClientObject(TcpClient tcpClient, ServerObject serverObject) {
            Id = Guid.NewGuid().ToString();
            client = tcpClient;
            server = serverObject;
            serverObject.AddConnection(this);
            Stream = client.GetStream();
        }

        public void Process() {
            try {
                
                // получаем имя пользователя
                string message = GetMessage();
                UserName = message;

                message = UserName + " подключился к игре";
                // посылаем сообщение о входе в чат всем подключенным пользователям
                //server.BroadcastMessage(message, this.Id);
                Console.WriteLine(message);
                // в бесконечном цикле получаем сообщения от клиента
                while (true) {
                    try {
                        message = GetMessage();
                        message = String.Format("{0}: {1}", UserName, message);
                        Console.WriteLine(message);
                        server.BroadcastMessage(message, this.Id);
                    } catch {
                        message = String.Format("{0}: покинул чат", UserName);
                        Console.WriteLine(message);
                        server.BroadcastMessage(message, this.Id);
                        break;
                    }
                }
            } catch (Exception e) {
                Console.WriteLine(e.Message);
            } finally {
                // в случае выхода из цикла закрываем ресурсы
                server.RemoveConnection(this.Id);
                Close();
            }
        }

        // чтение входящего сообщения и преобразование в строку
        private string GetMessage() {
            byte[] data = new byte[64]; // буфер для получаемых данных
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do {
                bytes = Stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (Stream.DataAvailable);

            return builder.ToString();
        }

        // закрытие подключения
        protected internal void Close() {
            if (Stream != null)
                Stream.Close();
            if (client != null)
                client.Close();
        }

    }
}