﻿using JT808.DotNetty.Client;
using JT808.Protocol.MessageBody;
using JT808.Protocol.Enums;
using JT808.Protocol.Extensions;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JT808.DotNetty.SimpleClient.Services
{
    public class UpService : IHostedService
    {
        private readonly IJT808TcpClientFactory jT808TcpClientFactory;

        public UpService(IJT808TcpClientFactory jT808TcpClientFactory)
        {
            this.jT808TcpClientFactory = jT808TcpClientFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            string sim =  "11111111111";
            JT808TcpClient client1 = jT808TcpClientFactory.Create(new JT808DeviceConfig(sim, "127.0.0.1", 808));
            string sim2 = "33333333333";
            JT808TcpClient client2 = jT808TcpClientFactory.Create(new JT808DeviceConfig(sim2, "127.0.0.1", 808));
            Thread.Sleep(5000);
            //1.终端注册
            client1.Send(JT808MsgId.终端注册.Create(sim, new JT808_0x0100()
            {
                PlateNo = "粤A12345",
                PlateColor = 2,
                AreaID = 0,
                CityOrCountyId = 0,
                MakerId = "Koike001",
                TerminalId = "Koike001",
                TerminalModel = "Koike001"
            }));
            client2.Send(JT808MsgId.终端注册.Create(sim2, new JT808_0x0100()
            {
                PlateNo = "粤A12345",
                PlateColor = 2,
                AreaID = 0,
                CityOrCountyId = 0,
                MakerId = "Koike001",
                TerminalId = "Koike001",
                TerminalModel = "Koike001"
            }));
            //2.终端鉴权
            client1.Send(JT808MsgId.终端鉴权.Create(sim, new JT808_0x0102()
            {
                Code = "1234"
            }));
            //2.终端鉴权
            client2.Send(JT808MsgId.终端鉴权.Create(sim2, new JT808_0x0102()
            {
                Code = "1234"
            }));
            Task.Run(() => {
                while (true)
                {
                    var i = 0;
                    //3.每5秒发一次
                    client1.Send(JT808MsgId.位置信息汇报.Create(sim, new JT808_0x0200()
                    {
                        Lat = 110000 + i,
                        Lng = 100000 + i,
                        GPSTime = DateTime.Now,
                        Speed = 50,
                        Direction = 30,
                        AlarmFlag = 5,
                        Altitude = 50,
                        StatusFlag = 10
                    }));
                    client2.Send(JT808MsgId.位置信息汇报.Create(sim2, new JT808_0x0200()
                    {
                        Lat = 110000 + i,
                        Lng = 100000 + i,
                        GPSTime = DateTime.Now,
                        Speed = 50,
                        Direction = 30,
                        AlarmFlag = 5,
                        Altitude = 50,
                        StatusFlag = 10
                    }));
                    i++;
                    Thread.Sleep(5000);
                }
            });
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
