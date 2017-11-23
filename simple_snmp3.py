from pysnmp.hlapi import *


__user_name = 'user'
__auth_pass = '0123456789'
__priv_pass = '1234567890'
__port = 161
__auth_protocol = usmHMACSHAAuthProtocol
__priv_protocol = usmAesCfb128Protocol


def __get_response(cmd):
    error_indication, error_status, error_index, var_binds = next(cmd)
    value = None

    if error_status:
        value = ('%s at %s' % (error_status.prettyPrint(),
                               error_index and var_binds[int(error_index) - 1][0] or '?'))
    elif var_binds and len(var_binds) > 0:
        value = var_binds[0][1]

    return {'errorIndication': error_indication,
            'errorStatus': error_status,
            'errorIndex': error_index,
            'varBinds': var_binds,
            'value': value}['value']


def get_oid(ip, oid):
    cmd = getCmd(SnmpEngine(),
                 UsmUserData(userName=__user_name,
                             authProtocol=__auth_protocol,
                             privProtocol=__priv_protocol,
                             authKey=__auth_pass,
                             privKey=__priv_pass),
                 UdpTransportTarget((ip, __port)),
                 ContextData(),
                 ObjectType(ObjectIdentity(oid)))
    return __get_response(cmd)


def set_oid(ip, oid, value):
    cmd = setCmd(SnmpEngine(),
                 UsmUserData(userName=__user_name,
                             authProtocol=__auth_protocol,
                             privProtocol=__priv_protocol,
                             authKey=__auth_pass,
                             privKey=__priv_pass),
                 UdpTransportTarget((ip, __port)),
                 ContextData(),
                 ObjectType(ObjectIdentity(oid), value))
    return __get_response(cmd)