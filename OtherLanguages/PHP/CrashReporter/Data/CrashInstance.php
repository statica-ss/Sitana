<?
class CrashInstance
{
	public $id;
	public $crashId;
	public $time;

	public function read($reader)
	{
		$this->id = $reader->hasField(0) ? $reader->readInteger(0) : null;
		$this->crashId = $reader->readInteger(1);
		$this->time = $reader->readStructure(2);
	}

	public function write($writer)
	{
		if ($this->id!=null) $writer->writeValue(0, $this->id);
		$writer->writeValue(1, $this->crashId);
		$writer->writeStructure(2, $this->time);
	}
}
?>