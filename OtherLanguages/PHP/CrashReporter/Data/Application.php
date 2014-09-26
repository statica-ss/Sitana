<?
class Application
{
	public $id;
	public $guid;
	public $name;

	public function read($reader)
	{
		$this->id = $reader->hasField(0) ? $reader->readInteger(0) : null;
		$this->guid = $reader->readString(1);
		$this->name = $reader->readString(2);
	}

	public function write($writer)
	{
		if ($this->id!=null) $writer->writeValue(0, $this->id);
		$writer->writeString(1, $this->guid);
		$writer->writeString(2, $this->name);
	}
}
?>