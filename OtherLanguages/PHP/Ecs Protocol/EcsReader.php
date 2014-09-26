<?php

require_once('BitConverterLE.php');

class EcsReader
{
    private $data;
    private $fields;
    private $size;
    
    public function __construct( $data, $offset, $size )
    {
        $this->data = $data;
        $this->size = $offset + $size;
        
        $fieldOffset = $offset;
        $id = $this->readField($offset);

        while ($id >= 0)
        {
            $this->fields[$id] = $fieldOffset + 1;
            $fieldOffset = $offset;

            $id = $this->readField($offset);
        }
    }
    
    public static function getStructureSize($data, $offset)
    {
        $oldOffset = $offset;
        $size = self::getNumBytesStatic($data, $offset);
        return $size + $offset - $oldOffset;
    }
    
    public function readString($id)
    {
        if (!isset($this->fields[$id]))
        {
            return null;
        }
        
        $offset = $this->fields[$id];

        $data = $this->data;
        $type = ord($data[$offset]);

        if ($type < 241 || $type > 244)
        {
            throw new Exception('Invalid field type.');
        }

        $bytes = $this->getNumBytes($offset);

        return $this->getString($this->data, $offset, $bytes);
    }
    
    public function readByteArray($id)
    {
        if (!isset($this->fields[$id]))
        {
            return null;
        }
        
        $offset = $this->fields[$id];
        
        $data = $this->data;
        $type = ord($data[$offset]);

        if ($type < 251 || $type > 254 )
        {
            throw new Exception('Invalid field type.');
        }

        $bytes = $this->getNumBytes($offset);

        $val = array();

        for ($idx = 0; $idx < $bytes; ++$idx)
        {
            $val[] = ord($data[$offset + $idx]);
        }

        return $val;
    }
    
    public function readStructure($id)
    {
        if (!isset($this->fields[$id]))
        {
            return null;
        }
        
        $offset = $this->fields[$id];
        
        $data = $this->data;
        
        $type = ord($data[$offset]);

        if ($type < 251 || $type > 254)
        {
            throw new Exception('Invalid field type. Byte=' . $type);
        }

        $bytes = $this->getNumBytes($offset);

        $reader = new EcsReader($data, $offset, $bytes);

        $name = $reader->ReadString(255);

        $val = new $name();

        $val->Read($reader);
        return $val;
    }
    
    public function readReal($id)
    {
        if (!isset($this->fields[$id]))
        {
            return null;
        }
        
        $offset = $this->fields[$id];
        
        $data = $this->data;
        $type = ord($data[$offset]);

        switch ($type)
        {
            case 1:
            case 2:
            case 3:
            case 4:
            case 17:
            case 18:
            case 19:
            case 20:
                return ReadInteger(id);

            case 36:
                return BitConverterLE::toSingle($data, $offset + 1);

            case 40:
                return BitConverterLE::toDouble($data, $offset + 1);
        }

        throw new Exception("Invalid field type.");
    }
    
    public function readInteger($id)
    {
        if (!isset($this->fields[$id]))
        {
            return null;
        }
        
        $offset = $this->fields[$id];
        
        $data = $this->data;
        $type = ord($data[$offset]);

        switch ($type)
        {
            case 1:
                return BitConverterLE::toInt8($data, $offset + 1);

            case 2:
                return BitConverterLE::toInt16($data, $offset + 1);
                
            case 3:
                return BitConverterLE::toInt24($data, $offset + 1);
                
            case 4:
                return BitConverterLE::toInt32($data, $offset + 1);

            case 17:
                return BitConverterLE::toUInt8($data, $offset + 1);

            case 18:
                return BitConverterLE::toUInt16($data, $offset + 1);
                
            case 19:
                return BitConverterLE::toUInt24($data, $offset + 1);

            case 20:
                return BitConverterLE::toUInt32($data, $offset + 1);
        }

        throw new Exception('Invalid field type.');
    }
    
    public function hasField($id)
    {
        return isset($this->fields[$id]);
    }
    
    private function getString(&$data, $offset, $length)
    {
        return substr($data, $offset, $length);
    }
    
    private function readSize($offset)
    {
        $data = $this->data;
        $type = ord($data[$offset]);

        if ($type == 0)
        {
            return 1;
        }

        if ($type > 240)
        {
            return $this->getNumBytesWithLength($offset);
        }

        $len = $type & 0xf;
        if ($len == 1 || $len == 2 || $len == 4 || $len == 3 || $type == 40)
        {
            return $len + 1;
        }

        throw new Exception('Unknown type of field. Type=' . $type);
    }

    private function readField(&$offset)
    {
        $data = $this->data;
        if ($offset >= $this->size)
        {
            return -1;
        }

        $id = ord($data[$offset]);

        $offset++;

        $size = $this->readSize($offset);
        $offset += $size;

        if ($offset <= $this->size)
        {
            return $id;
        }

        return -1;
    }
    
    private static function getNumBytesStatic(&$array, &$offset)
    {
        $bytes = -1;

        $type = ord($array[$offset]);

        switch ($type)
        {
            case 241:
            case 251:
                $bytes = BitConverterLE::toUInt8($array, $offset + 1);
                $offset += 2;
                break;

            case 242:
            case 252:
                $bytes = BitConverterLE::toUInt16($array, $offset + 1);
                $offset += 3;
                break;
                
            case 243:
            case 253:
                $bytes = BitConverterLE::toUInt24($array, $offset + 1);
                $offset += 4;
                break;

            case 244:
            case 254:
                $bytes = BitConverterLE::toInt32($array, $offset + 1);
                $offset += 5;
                break;
        }

        if ($bytes < 0)
        {
            throw new Exception('Invalid field type. Byte=' . $type);
        }

        return $bytes;
    }

    private function getNumBytes(&$offset)
    {
        return self::getNumBytesStatic($this->data, $offset);
    }

    private function getNumBytesWithLength($offset)
    {
        return self::getStructureSize($this->data, $offset);
    }
}

?>