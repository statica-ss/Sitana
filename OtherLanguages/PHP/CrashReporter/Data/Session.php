<?
class Session
{
	public $id;
	public $applicationGuid;
	public $deviceGuid;
	public $time;

	public function read($reader)
	{
		$this->id = $reader->hasField(0) ? $reader->readInteger(0) : null;
		$this->applicationGuid = $reader->readString(1);
		$this->deviceGuid = $reader->readString(2);
		$this->time = $reader->readStructure(3);
	}

	public function write($writer)
	{
		if ($this->id!=null) $writer->writeValue(0, $this->id);
		$writer->writeString(1, $this->applicationGuid);
		$writer->writeString(2, $this->deviceGuid);
		$writer->writeStructure(3, $this->time);
	}
}
?>