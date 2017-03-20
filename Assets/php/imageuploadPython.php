<?php

   if(isset($_FILES['image']))
   {
   move_uploaded_file($_FILES['image']['tmp_name'], realpath(dirname(getcwd())) . '/images/' . $_FILES['image']['name']);
       
       require_once(realpath(dirname(getcwd())) . '/php/VuforiaClient.php');
        $client = new VuforiaClient();
        $client->addTarget($_FILES['image']['name'],realpath(dirname(getcwd())) . '/images/' . $_FILES['image']['name'],$_FILES['meta']);
    
   } else
   {
      print("Failed!");
   }
?>