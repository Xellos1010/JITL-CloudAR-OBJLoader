<?php
require_once 'VuforiaClient.php';

$client = new VuforiaClient();

if( isset( $_GET['select']) ){
	
	$selection = $_GET['select'];

	switch( $selection ){
		case "AddTarget" :
            $client->addTarget($_GET['name'],$_GET['image'],$_GET['meta']);
            break;
		case "GetTargetInfo" :
			$client->getTargetInfo($_GET['targetID']);
			break;
        case "GetTargetSummary" :
			$client->getTargetSummary($_GET['targetID']);
			break;
        case "GetAllTargets" :            
			return $client->getAllTargets();
		case "UpdateTarget" :
			$instance = new UpdateTarget();
			break;
		case "DeleteTarget" :
			$client->deleteTarget($_GET['targetID']);
			break;
		case "DeleteAllTargets" :
			$client->DeleteAllTargets();
			break;
		default :
			echo "INVALID SELECTION";
			break;
		
	}
	
	echo "</div>";
	
	
	echo "<br /><div>~~~~~~~~~~~~~~~</div><br />";
	
}
elseif( isset( $_POST['select']) ){
	
	$selection = $_POST['select'];
	
	switch( $selection ){
		case "AddTarget" :
            $client->addTarget($_POST['name'],$_POST['image'],$_POST['meta']);
            break;
		case "GetTargetInfo" :
			$client->getTargetInfo($_POST['targetID']);
			break;
        case "GetTargetSummary" :
			$client->getTargetSummary($_POST['targetID']);
			break;
        case "GetAllTargets" :
			return $client->getAllTargets();
		case "UpdateTarget" :
			$instance = new UpdateTarget();
			break;
		case "DeleteTarget" :
			$client->deleteTarget($_POST['targetID']);
			break;
		case "DeleteAllTargets" :
			$client->DeleteAllTargets();
			break;
        case "PostNewTarget" :
			$instance = new PostNewTarget();
			break;
		default :
			echo "INVALID SELECTION";
			break;
		
	}
}
?>