<?
class Crash
{
	public $id;
	public $applicationGuid;
	public $exception;

	public function read($reader)
	{
		$this->id = $reader->hasField(0) ? $reader->readInteger(0) : null;
		$this->applicationGuid = $reader->readString(1);
		$this->exception = $reader->readString(2);
	}

	public function write($writer)
	{
		if ($this->id!=null) $writer->writeValue(0, $this->id);
		$writer->writeString(1, $this->applicationGuid);
		$writer->writeString(2, $this->exception);
	}
}
?>