﻿using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hbm.Devices.Jet;
using Newtonsoft.Json.Linq;

namespace Hbm.Automation.Api.Weighing.WTX.Jet
{
    public class JetBusConnection : INetConnection, IDisposable
    {
        #region ==================== constants & fields ====================

        private const string STD_WTX_AUTHENTICATION_USER = "Administrator";
        private const string STD_WTX_AUTHENTICATION_PASSWORD = "wtx";
        private readonly JetPeer _peer;
        private readonly AutoResetEvent _successEvent = new AutoResetEvent(false);
        private Exception _localException;
        private string _password;
        private string _user;
        private int _timeoutMs;
        private bool _disposedValue;

        #endregion

        #region ==================== events & delegates ====================

        public event EventHandler<LogEventArgs> CommunicationLog;

        public event EventHandler<EventArgs> UpdateData;

        #endregion

        #region =============== constructors & destructors =================

        /// <summary>
        ///     Initializes a new instance of the <see cref="JetBusConnection" /> class.
        ///     Use this constructor for individual authentication
        /// </summary>
        /// <param name="ipAddress">IP address of the target device</param>
        /// <param name="user">User for device authentication</param>
        /// <param name="password">Password for device authentication</param>
        public JetBusConnection(string ipAddress, string user, string password)
        {
            var _uri = "wss://" + ipAddress + ":443/jet/canopen";

            IJetConnection jetConnection = new WebSocketJetConnection(_uri, RemoteCertificationCheck);
            _peer = new JetPeer(jetConnection);

            _user = user;
            _password = password;
            IpAddress = ipAddress;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="JetBusConnection" /> class.
        ///     Use this constructor for authentication with standard credentials
        /// </summary>
        /// <param name="ipAddress">IP address of the target device</param>
        public JetBusConnection(string ipAddress) : this(ipAddress, STD_WTX_AUTHENTICATION_USER,
                                                         STD_WTX_AUTHENTICATION_PASSWORD)
        {
        }

        #endregion

        #region ======================== properties ========================

        /// <inheritdoc />
        public ConnectionType ConnectionType =>
                ConnectionType.Jetbus;

        /// <inheritdoc />
        public bool IsConnected { get; private set; }

        /// <inheritdoc />
        public string IpAddress { get; set; }

        /// <inheritdoc />
        public Dictionary<string, string> AllData { get; } = new Dictionary<string, string>();

        #endregion

        #region ================ public & internal methods =================

        /// <inheritdoc />
        public void Connect(int timeoutMs = 20000)
        {
            IsConnected = false;
            _timeoutMs = timeoutMs;
            ConnectPeer(_user, _password, timeoutMs);

            // We wait until all is done:
            // ConnectPeer
            //  OnConnectAuthenticate
            //      OnAuthenticateFetchAll
            //          FetchAll
            WaitOne(3);
        }

        /// <inheritdoc />
        public void Disconnect()
        {
            if (_peer != null)
            {
                try
                {
                    _peer.Disconnect();
                }
                catch (JetPeerException)
                {
                }
            }

            IsConnected = false;
        }

        /// <inheritdoc />
        public string Read(object command)
        {
            return ReadFromBuffer(command);
        }

        /// <inheritdoc />
        public Task<string> ReadAsync(object command)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public string ReadFromBuffer(object command)
        {
            var jetcommand = (JetBusCommand)command;
            return jetcommand.ToString(AllData[jetcommand.Path]);
        }

        /// <inheritdoc />
        public string ReadFromDevice(object command)
        {
            return "";
        }

        /// <inheritdoc />
        public int ReadIntegerFromBuffer(object command)
        {
            var jetcommand = (JetBusCommand)command;
            return jetcommand.ToSValue(AllData[jetcommand.Path]);
        }

        /// <inheritdoc />
        public bool WriteInteger(object command, int value)
        {
            var result = false;
            var jasonValue = new JValue(value);
            var _command = (JetBusCommand)command;
            SetData(_command.Path, jasonValue);
            WaitOne();
            result = true;
            return result;
        }

        /// <inheritdoc />
        public bool Write(object command, string value)
        {
            var result = false;
            var jasonValue = new JValue(value);
            var _command = (JetBusCommand)command;
            SetData(_command.Path, jasonValue);
            WaitOne();
            result = true;
            return result;
        }

        /// <inheritdoc />
        public Task<int> WriteAsync(object command, int commandParam)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);

            // uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        #endregion

        #region =============== protected & private methods ================

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _successEvent.Close();
                    _successEvent.Dispose();
                }

                _disposedValue = true;
            }
        }

        private void ConnectPeer(string user, string password, int timeoutMs)
        {
            _user = user;
            _password = password;
            _peer.Connect(OnConnectAuthenticate, timeoutMs);
        }

        private void OnConnectAuthenticate(bool connected)
        {
            if (connected)
            {
                _peer.Authenticate(_user, _password, OnAuthenticateFetchAll, _timeoutMs);
                CommunicationLog?.Invoke(this, new LogEventArgs("Connection successful"));
            }
            else
            {
                _localException = new Exception("Connection failed");
                _successEvent.Set();
            }
        }

        private void OnAuthenticateFetchAll(bool authenticationSuccess, JToken token)
        {
            if (authenticationSuccess)
            {
                CommunicationLog?.Invoke(this, new LogEventArgs("Authentication successful"));
                FetchAll();
            }
            else
            {
                var exception = new JetBusException(token);
                _localException = new Exception(exception.Message);
                _successEvent.Set();
            }
        }

        private void FetchAll()
        {
            var matcher = new Matcher();
            FetchId id;
            _peer.Fetch(out id, matcher, OnFetchData, OnFetch, _timeoutMs);
        }

        private void OnFetch(bool success, JToken token)
        {
            if (success)
            {
                IsConnected = true;
            }
            else
            {
                var exception = new JetBusException(token);
                _localException = new Exception(exception.ErrorCode.ToString());
            }

            _successEvent.Set();

            UpdateData?.Invoke(this, new EventArgs());

            CommunicationLog?.Invoke(this,
                                     new LogEventArgs("Fetch-All success: " +
                                                      success +
                                                      " - Buffer size is " +
                                                      AllData.Count));
        }

        private void WaitOne(int timeoutMultiplier = 1)
        {
            if (!_successEvent.WaitOne(_timeoutMs * timeoutMultiplier))
            {
                throw new Exception("Jet connection timeout");
            }

            // if (_localException != null)
            // {
            //     CommunicationLog?.Invoke(this, new LogEventArgs(_localException.Message));
            //     Exception exception = _localException;
            //     _localException = null;
            //     throw exception;
            // }
        }

        /// <summary>
        ///     Event will be called when device sends new fetch events
        /// </summary>
        /// <param name="data">New device data from jet peer</param>
        private void OnFetchData(JToken data)
        {
            var path = data["path"]
                    .ToString();

            lock (AllData)
            {
                switch (data["event"]
                                .ToString())
                {
                    case "add":
                        AllData.Add(path, data["value"]
                                            .ToString());
                        break;

                    case "fetch":
                        AllData[path] = data["value"]
                                .ToString();
                        break;

                    case "change":
                        AllData[path] = data["value"]
                                .ToString();
                        break;
                }

                if (IsConnected)
                {
                    UpdateData?.Invoke(this, new EventArgs());
                }

                CommunicationLog?.Invoke(this, new LogEventArgs(data.ToString()));
            }
        }

        /// <summary>
        ///     Sets data for a single jet path
        /// </summary>
        /// <param name="path">Jet path for the data (e.g. "6002/01)"</param>
        /// <param name="value">The new value for the path</param>
        private void SetData(string path, JValue value)
        {
            _localException = null;
            try
            {
                var request = _peer.Set(path, value, OnSet, _timeoutMs);
            }
            catch (Exception e)
            {
                _successEvent.Set();
                _localException = e;
            }
        }

        private void OnSet(bool success, JToken token)
        {
            if (!success)
            {
                _localException = new JetBusException(token);
            }

            _successEvent.Set();

            CommunicationLog?.Invoke(this, new LogEventArgs("Set data" + success));
        }

        /// <summary>
        ///     Callback-Method wich is called from SslStream for SSL certificate validation
        /// </summary>
        /// <param name="sender">Sender holding the SSL stream</param>
        /// <param name="certificate">The SSL certificate to be validated</param>
        /// <param name="chain">SSL certification chain</param>
        /// <param name="sslPolicyErrors">Any policy violations</param>
        /// <returns>Indicates if validation was successful or not</returns>
        private bool RemoteCertificationCheck(object sender, X509Certificate certificate, X509Chain chain,
                                              SslPolicyErrors sslPolicyErrors)
        {
            try
            {
                var clientCertificate = new X509Certificate2(CertificateToByteArray(), string.Empty);
                var sslStream = sender as SslStream;
                if (sslPolicyErrors == SslPolicyErrors.None ||
                    sslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors)
                {
                    foreach (var item in chain.ChainElements)
                    {
                        item.Certificate.Export(X509ContentType.Cert);

                        // If one of the included status-flags is not posiv then the cerficate-check
                        // failed. Except the "untrusted root" because it is a self-signed certificate
                        foreach (var status in item.ChainElementStatus)
                        {
                            if (status.Status != X509ChainStatusFlags.NoError &&
                                status.Status != X509ChainStatusFlags.UntrustedRoot &&
                                status.Status != X509ChainStatusFlags.NotTimeValid)
                            {
                                return false;
                            }
                        }

                        // compare the certificate in the chain-collection. If on of the certificate at
                        // the path to root equal, are the check ist positive
                        if (clientCertificate.Equals(item.Certificate))
                        {
                            return true;
                        }
                    }
                }

                // TODO: to reactivate the hostename-check returning false.
                return true;
            }
            catch (Exception)
            {
                // If thrown any exception then is the certification-check failed
                return true;
            }
        }

        private byte[] CertificateToByteArray()
        {
            const string CERTIFICATE =
                    "MIIECzCCAvOgAwIBAgIJAPTJN5RGpzbRMA0GCSqGSIb3DQEBBQUAMIGbMQswCQYDVQQGEwJERTELMAkGA1UECAw" +
                    "CSEUxEjAQBgNVBAcMCURhcm1zdGFkdDErMCkGA1UECgwiSG90dGluZ2VyIEJhbGR3aW4gTWVzc3RlY2huaWsgR21iSDELMAkGA1UECwwCV" +
                    "1QxFDASBgNVBAMMC3d3dy5oYm0uY29tMRswGQYJKoZIhvcNAQkBFgxpbmZvQGhibS5jb20wHhcNMTcwNDA2MTU1NzQzWhcNMjcwNDA0MTU1N" +
                    "zQzWjCBmzELMAkGA1UEBhMCREUxCzAJBgNVBAgMAkhFMRIwEAYDVQQHDAlEYXJtc3RhZHQxKzApBgNVBAoMIkhvdHRpbmdlciBCYWxkd2luIE1" +
                    "lc3N0ZWNobmlrIEdtYkgxCzAJBgNVBAsMAldUMRQwEgYDVQQDDAt3d3cuaGJtLmNvbTEbMBkGCSqGSIb3DQEJARYMaW5mb0BoYm0uY29tMIIBIjA" +
                    "NBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAr + 51SnSoQX1M3aOUwaD8dcFIoac9peaiRsIOUqwJGn58g + n53aevw54sFyvffcJnzZVAFEP" +
                    "Ech1oQCNowsNDnNr4UL / NaO4C4GsJX5bmdia6nGj7IWLMeQzs + 0gfqPWmO / OsJVnwrp9h6 / SxuIz5n04l7ESupSBnXfifb9RGA0encHt" +
                    "ZK0LxHRD9sxyhNYKDKW76hgDK / EZ5HU2YjKS0y58 + PU15AV + vQ5srJFMC + KNHveWF4xgj528r3C25FWpVtW5Dqd937OrSAS5truGxPBz" +
                    "enWx3PHw6zRPvBbOApTNWLfbcp90mF8 / 9wFi94PG + EokaYSoF0xyT2G6fAVs3qQIDAQABo1AwTjAdBgNVHQ4EFgQUZC39SAkffhRee1x / 7" +
                    "TbnrQJQ / jMwHwYDVR0jBBgwFoAUZC39SAkffhRee1x / 7TbnrQJQ / jMwDAYDVR0TBAUwAwEB / zANBgkqhkiG9w0BAQUFAAOCAQEACRI28" +
                    "UaB6KNtDVee + waz + SfNd3tm / RspwRarJBlf9dcbpcxolvp9UxCoWkyf9hkmJPEyJzJltuan08DkqmschD36saOJcVRh6BfLNBD8DkFTavP" +
                    "0Q2BKb8FvJpdacNTRK542sJHSk5gr6imwnD5EAj + OT24UpH5rwq5Esu5TYFLdSuYfRXw6puTION / fqqTKVK9Au0TdFPgZ4Fppym4fInQ0jJQ" +
                    "hcGSWMs3yomPqftwitRwv5 / p8hLtf3yNIkk9OnBwPpT7QxXxw4Zs0Jvl / VBFuNwbeD12ur3RKbMyCn9W0RjaMrYpKnAjik3IlSqDYZ0XDMwZ" +
                    "0oQiOFy / a6bR4Vw ==";
            var _byteArray = Encoding.ASCII.GetBytes(CERTIFICATE);

            return _byteArray;
        }

        #endregion
    }
}
