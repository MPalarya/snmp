from simple_snmp3 import *


response = set_oid(ip='150.42.40.221',
                   oid='1.3.6.1.4.1.16215.1.18.1.30.7.1.3.0',
                   value=Integer32(1))

response = set_oid(ip='150.42.40.221',
                   oid='1.3.6.1.4.1.16215.1.18.1.30.7.1.3.0',
                   value=OctetString('BlaBla'))

response = get_oid(ip='150.42.40.221',
                   oid='1.3.6.1.4.1.16215.1.18.1.4.6.0')

print response
