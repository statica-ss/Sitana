<?php

require_once('BitConverterLE.php');

class EcsWriter
{
    private $data;

    public function __construct()
    {
        $data = '';
    }
    
    public function getData()
    {
        return $this->data;
    }

    public function writeRaw($val)
    {
        $this->data .= $val;
    }

    private function writeInt8($id, $val)
    {
        $this->data .= chr($id);
        $this->data .= chr(1);
        BitConverterLE::addBytesInt8($this->data, $val);
    }

    private function writeInt16($id, $val)
    {
        if ( abs($val) < (1<<7))
        {
            $this->writeInt8($id, $val);
        }
        else
        {
            $this->data .= chr($id);
            $this->data .= chr(2);
            BitConverterLE::addBytesInt16($this->data, $val);
        }
    }
    
    private function writeInt24($id, $val)
    {
        if ( abs($val) < (1<<15))
        {
            $this->writeInt16($id, $val);
        }
        else
        {
            $this->data .= chr($id);
            $this->data .= chr(3);
            BitConverterLE::addBytesInt24($this->data, $val);
        }
    }
    
    private function writeInt32($id, $val)
    {
        if ( abs($val) < (1<<23))
        {
            $this->writeInt24($id, $val);
        }
        else
        {
            $this->data .= chr($id);
            $this->data .= chr(4);
            BitConverterLE::addBytesInt32($this->data, $val);
        }
    }
    
    private function writeUInt8($id, $val)
    {
        $this->data .= chr($id);
        $this->data .= chr(17);
        BitConverterLE::addBytesUInt8($this->data, $val);
    }

    private function writeUInt16($id, $val)
    {
        if ( $val < (1<<8))
        {
            $this->writeInt8($id, $val);
        }
        else
        {
            $this->data .= chr($id);
            $this->data .= chr(18);
            BitConverterLE::addBytesUInt16($this->data, $val);
        }
    }
    
    private function writeUInt24($id, $val)
    {
        if ( $val < (1<<16))
        {
            $this->writeInt16($id, $val);
        }
        else
        {
            $this->data .= chr($id);
            $this->data .= chr(19);
            BitConverterLE::addBytesUInt24($this->data, $val);
        }
    }
    
    private function writeUInt32($id, $val)
    {
        if ( $val < (1<<24))
        {
            $this->writeUInt24($id, $val);
        }
        else
        {
            $this->data .= chr($id);
            $this->data .= chr(20);
            BitConverterLE::addBytesUInt32($this->data, $val);
        }
    }

    private function writeDouble($id, $val)
    {
        $this->data .= chr($id);
        $this->data .= chr(40);
        
        BitConverterLE::addBytesDouble($this->data, $val);
    }
    
    public function writeValue($id, $value)
    {
        if (is_float($value))
        {
            $this->writeDouble($id, $value);
        }
        else
        {
            $value = intval($value);
            
            if ( $value < 0 )
            {
                $this->writeInt32($id, $value);
            }
            else
            {
                $this->writeUInt32($id, $value);
            }
        }
    }

    public function writeString($id, $value)
    {
        $len = mb_strlen($value, '8bit');
        
        $this->data .= chr($id);
        
        $this->writeTypeAndLength(240, $len);
        
        $this->data .= $value;
    }

    
    public function writeStructure($id, $val)
    {
        $writer = new EcsWriter();
        
        $writer->writeString(255, get_class($val));
        
        $val->write($writer);

        $this->writeByteArray($id, $writer->GetData());
    }
    
    public function writeByteArray($id, &$value)
    {
        $len = mb_strlen($value, '8bit');
        
        $this->data .= chr($id);
        
        $this->writeTypeAndLength(250, $len);
        
        $this->data .= $value;
    }
    
    private function writeTypeAndLength($type0, $len)
    {
        if ( $len < (1<<8) )
        {
            $this->data .= chr($type0+1);
            
            BitConverterLE::addBytesUInt8($this->data, $len);
        }
        else if ( $len < (1<<16))
        {
            $this->data .= chr($type0+2);
            
            BitConverterLE::addBytesUInt16($this->data, $len);
        }
        else if ( $len < (1<<24))
        {
            $this->data .= chr($type0+3);
            
            BitConverterLE::addBytesUInt24($this->data, $len);
        }
        else
        {
            $this->data .= chr($type0+4);
            
            BitConverterLE::addBytesUInt32($this->data, $len);
        }
    }
}
?>