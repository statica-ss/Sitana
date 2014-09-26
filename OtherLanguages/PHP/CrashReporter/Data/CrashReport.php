<?
class CrashReport
{
	public $crash;
	public $time;

	public function read($reader)
	{
		$this->crash = $reader->readString(0);
		$this->time = $reader->readStructure(1);
	}

	public function write($writer)
	{
		$writer->writeString(0, $this->crash);
		$writer->writeStructure(1, $this->time);
	}
}
?>