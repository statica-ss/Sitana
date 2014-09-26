<?php
require_once('EcsProtocol.php');
require_once('TestClasses.php');

$filename = "test.bin";
$handle = fopen($filename, "r");
$buf = fread($handle, filesize($filename));
fclose($handle);

$obj = ecsRead($buf);

$content = ecsWrite($obj);

$filename = "test.out";
$handle = fopen($filename, "w");
fwrite($handle, $content);
fclose($handle);

echo $obj;
?>