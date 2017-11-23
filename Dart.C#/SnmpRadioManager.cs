using Dart.Snmp;
using SnmpManager.Properties;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using IPEndPoint = System.Net.IPEndPoint;

namespace SnmpManager
{
    public class SnmpRadioManager : ISnmpRadioManager
    {
        #region Fields
        private static Manager _manager;
        private bool _isOpen = false;
        #endregion Fields

        #region Constructors
        public SnmpRadioManager()
        {

        }
        #endregion Constructors

        #region Properties
        private IPEndPoint DeviceEndPoint;
        private string Community;
        private SnmpVersion Version;
        private User User; // Provides SNMPv3 User details
        #endregion Properties

        #region Interface Methods
        public bool OpenSession(IPAddress deviceIp, short port, string community, int version, string username, string authPass, string privPass)
        {
            try
            {
                DeviceEndPoint = new IPEndPoint(deviceIp, port);
                Community = community;
                Version = GetVersionByInt(version);

                if (Version == SnmpVersion.Three)
                    // TODO: Remote Operator Panel shall encrypt in Aes256.
                    User = new User(username, authPass, AuthenticationProtocol.Sha, privPass, PrivacyProtocol.Aes128);

                _manager = new Manager();
                using (var mibFile = new FileStream(Settings.Default.MIBPath, FileMode.Open, FileAccess.Read))
                {
                    _manager.Mib.Parse(mibFile);
                    _manager.Mib.GenerateNodes();
                }

                _manager.Security.TrapUsers.Add(User);
                _manager.Start(new NotificationReceived(TrapReceived), new Dart.Snmp.IPEndPoint("", 162), true, null);

                _isOpen = true;
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public object Get(string varName, int timeout)
        {
            var socket = new SnmpSocket(_manager);
            var request = new GetMessage();

            BuildRequest(request, new Variable(_manager.Mib[varName]));

            var task = Task.Run(() => socket.GetResponse(request, DeviceEndPoint));
            if (task.Wait(timeout))
                // TODO: make sure that the response is returned in the first [0] element and not in the last
                return task.Result.Variables[0].Value as object;

            return null;
        }

        public bool Set(string varName, object value, int timeout)
        {
            var socket = new SnmpSocket(_manager);
            var request = new SetMessage();

            BuildRequest(request, new Variable(_manager.Mib[varName], value.ToString()));

            var task = Task.Run(() => socket.GetResponse(request, DeviceEndPoint));
            if (task.Wait(timeout))
                return task.Result.ErrorStatus == ErrorStatus.Success;

            return false;
        }

        public bool IsOpen()
        {
            return _isOpen;
        }
        #endregion Interface Methods

        #region Helper Methods
         private void TrapReceived(Manager manager, MessageBase receivedMessage, object state)
         {
             manager.Send(new ResponseMessage(receivedMessage as InformMessage, null), receivedMessage.Origin);
         }

        private SnmpVersion GetVersionByInt(int versionNumber)
        {
            switch (versionNumber)
            {
                default:
                case 1:
                    return SnmpVersion.One;
                case 2:
                    return SnmpVersion.Two;
                case 3:
                    return SnmpVersion.Two;
            }
        }

        private void BuildRequest(RequestMessage request, Variable variable)
        {
            request.Version = Version;
            request.Community = Community;
            request.Origin = DeviceEndPoint;
            request.Security.User = User;
            request.Variables.Add(variable);
        }
        #endregion Helper Methods
    }
}
