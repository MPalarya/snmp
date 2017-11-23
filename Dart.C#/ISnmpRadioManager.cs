using System.Net;

namespace SnmpManager
{
    /// <summary>
    /// Interface to define the methods used by the Web Server to communicate with the Radio Device over SNMP protocol
    /// </summary>
    public interface ISnmpRadioManager
    {
        /// <summary>
        /// Function will open a new SNMP communication session,
        /// The cryptographic properties are pre-defined in the radio and cannot be specified
        /// Parameters username, authPass, privPass will be ignored on SNMP version lower than SNMPv3
        /// </summary>
        /// <param name="deviceIp">ip address</param>
        /// <param name="port">port number</param>
        /// <param name="community">SNMP community string</param>
        /// <param name="version">SNMP version</param>
        /// <param name="username">SNMPv3 username</param>
        /// <param name="authPass">SNMPv3 authentication password</param>
        /// <param name="privPass">SNMPv3 private password</param>
        /// <returns>true/false for operation success</returns>
        bool OpenSession(IPAddress deviceIp, short port, string community, int version, string username, string authPass, string privPass);

        /// <summary>
        /// Function shall Set a given value to a specified OID in the MIBtree based on an SNMP protocol
        /// </summary>
        /// <param name="attributeName">Name of the attribute in the MIBtree</param>
        /// <param name="value">Value of any supported type</param>
        /// <param name="timeout">Timeout in miliseconds in case of no reponse</param>
        /// <returns>true/false for operation success</returns>
        bool Set(string attributeName, object value, int timeout);

        /// <summary>
        /// Function shall Get a value of a specified OID in the MIBtree based on an SNMP protocol
        /// </summary>
        /// <param name="attributeName">Name of the attribute in the MIBtree</param>
        /// <param name="timeout">Timeout in miliseconds in case of no reponse</param>
        /// <returns>Value of any supported type</returns>
        object Get(string attributeName, int timeout);

        /// <summary>
        /// Returns if OpenSession was called successfully
        /// </summary>
        /// <returns>true/false</returns>
        bool IsOpen();
    }
}
