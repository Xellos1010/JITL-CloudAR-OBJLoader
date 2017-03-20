<?php
   print($_FILES['meta']);
   if(isset($_FILES['theFile']))
   {
   move_uploaded_file($_FILES['theFile']['tmp_name'], realpath(dirname(getcwd())) . '/images/' . $_FILES['theFile']['name']);
       
       require_once(realpath(dirname(getcwd())) . '/php/VuforiaClient.php');
        $client = new VuforiaClient();
        $client->addTarget($_FILES['theFile']['name'],realpath(dirname(getcwd())) . '/images/' . $_FILES['theFile']['name'],$_POST['meta']);
        //print($_FILES['meta']);
        echo realpath(dirname(getcwd())) . '/images/' . $_FILES['theFile']['name'];
   } else {
      print("Failed!");
   }
?>