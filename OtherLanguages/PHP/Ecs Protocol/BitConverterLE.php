<?php
class BitConverterLE
{
    const LITTLE_ENDIAN_ORDER = 1;
    const BIG_ENDIAN_ORDER = 2;
    
    static $endianess = 0;
    
    private static function isBigEndian()
    {
        if(self::$endianess == 0)
        {
            list(,$val) = unpack('S',"\x01\x00");
            
            if ( $val == 1 )
            {
                self::$endianess = self::LITTLE_ENDIAN_ORDER;
            }
            else
            {
                self::$endianess = self::BIG_ENDIAN_ORDER;
            }
        }
        
        return self::$endianess == self::BIG_ENDIAN_ORDER;
    }
    
    public static function toDouble(&$data, $offset)
    {
        $buffer = substr($data, $offset, 8);
        
        if(self::isBigEndian())
        {
            $buffer = strrev($buffer);
        }
        
        list(,$val) = unpack('d', $buffer);
        return $val;
    }

    public static function toSingle(&$data, $offset)
    {
        $buffer = substr($data, $offset, 4);
        
        if(self::isBigEndian())
        {
            $buffer = strrev($buffer);
        }
        
        list(,$val) = unpack('f', $buffer);
        return $val;
    }
    
    public static function toUInt8(&$data, $offset)
    {
        list(,$val) = unpack('C', substr($data, $offset, 1));
        return $val;
    }

    public static function toInt8(&$data, $offset)
    {
        list(,$val) = unpack('c', substr($data, $offset, 1));
        return $val;
    }
    
    public static function toUInt16(&$data, $offset)
    {
        list(,$val) = unpack('v', substr($data, $offset, 2));
        return $val;
    }

    public static function toInt16(&$data, $offset)
    {
        $bytes = substr($data, $offset, 2);
        
        if(self::isBigEndian())
        {
            $bytes = strrev($bytes);
        }
    
        list(,$val) = unpack('s', $bytes);
        return $val;
    }
    
    public static function toUInt24(&$data, $offset)
    {
        list(,$val) = unpack('V', substr($data, $offset, 3) . "\x00");
        return $val;
    }

    public static function toInt24(&$data, $offset)
    {
        list(,$b0,$b1,$b2) = unpack('C*', substr($data, $offset, 3));
        
        $val = $b0 | ($b1 << 8) | ($b2 << 16);
        
        if( ($b2&0x80) != 0 )
        {
            $val = $val - (1<<24);
        }
        
        return $val;
    }

    public static function toUInt32(&$data, $offset)
    {
        list(,$val) = unpack('V', substr($data, $offset, 4));
        
        return $val;
    }

    public static function toInt32(&$data, $offset)
    {
        $bytes = substr($data, $offset, 4);
        
        if(self::isBigEndian())
        {
            $bytes = strrev($bytes);
        }
        
        list(,$val) = unpack('i', $bytes);
        
        return $val;
    }

    public  static function addBytesUInt32(&$data, $val)
    {
        $bytes = pack('L', $val);
        self::addBytes($data, $bytes);
    }
    
    public  static function addBytesInt32(&$data, $val)
    {
        $bytes = pack('l', $val);
        self::addBytes($data, $bytes);
    }

    public  static function addBytesUInt24(&$data, $val)
    {
        $bytes = pack('V', $val);
        $data .= substr($bytes, 0, 3);
    }
    
    public  static function addBytesInt24(&$data, $val)
    {
        if ( $val < 0 )
        {
            $val = $val | 0x800000;
        }
        else
        {
            $val = $val & 0x7fffff;
        }
    
        $bytes = pack('l', $val);
    
        if(self::isBigEndian())
        {
            $bytes = strrev($bytes);
        }
    
        $data .= substr($bytes, 0, 3);
    }
    
    public  static function addBytesUInt16(&$data, $val)
    {
        $bytes = pack('S', $val);
        self::addBytes($data, $bytes);
    }
    
    public  static function addBytesInt16(&$data, $val)
    {
        $bytes = pack('s', $val);
        self::addBytes($data, $bytes);
    }
    
    public  static function addBytesInt8(&$data, $val)
    {
        $data .= pack('c', $val);
    }
    
    public static function addBytesUInt8(&$data, $val)
    {
        $data .= pack('C', $val);
    }

    public static function addBytesSingle(&$data, $val)
    {
        $bytes = pack('f', $val);
        self::addBytes($data, $bytes);
    }
    
    public static function addBytesDouble(&$data, $val)
    {
        $bytes .= pack('d', $val);
        self::addBytes($data, $bytes);
    }

    private static function addBytes(&$data, $bytes)
    {
        if(self::isBigEndian())
        {
            $data .= strrev($bytes);
        }
        else
        {
            $data .= $bytes;
        }
    }
}
?>