<?php

class Date
{
    public $Year;
    public $Month;
    public $Day;

    public function read($reader)
    {
        $this->Year = $reader->ReadInteger(0);
        $this->Month = $reader->ReadInteger(1);
        $this->Day = $reader->ReadReal(2);
    }

    public function write($writer)
    {
        $writer->WriteValue(0, $this->Year);
        $writer->WriteValue(1, $this->Month);
        $writer->WriteValue(2, $this->Day);
    }

    public function __toString()
    {
        return sprintf("%04d-%02d-%02d", $this->Year, $this->Month, $this->Day);
    }
}

class Person
{
    public $Name;
    public $LastName;
    public $BirthDate;

    public function read($reader)
    {
        $this->Name = $reader->ReadString(0);
        $this->LastName = $reader->ReadString(1);
        $this->BirthDate = $reader->ReadStructure(2);
    }

    public function write($writer)
    {
        $writer->WriteString(0, $this->Name);
        $writer->WriteString(1, $this->LastName);
        $writer->WriteStructure(2, $this->BirthDate);
    }

    public function __toString()
    {
        return 'Name='.$this->Name.', LastName='.$this->LastName.', BirthDate=' . $this->BirthDate->__toString();
    }
}

class DDD
{
    public $Persons = array();

    public function read($reader)
    {
        $count = $reader->ReadInteger(0);
        $this->Persons = array();

        for ($idx = 0; $idx < $count; ++$idx)
        {
            $this->Persons[] = $reader->ReadStructure($idx + 1);
        }
    }

    public function write($writer)
    {
        $count = count($this->Persons);
        $writer->WriteValue(0, $count);

        $idx = 1;
        foreach($this->Persons as $person)
        {
            $writer->WriteStructure($idx, $person);
            $idx++;
        }
    }

    public function __toString()
    {
        $ret = '';

        foreach ($this->Persons as $person)
        {
            if ($ret != '')
            {
                $ret .= '<br>';
            }

            $ret .= $person->__toString();
        }

        return $ret;
    }
}

?>