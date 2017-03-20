<?php
/**
 * Created by PhpStorm.
 * User: jacob
 * Date: 8/12/14
 * Time: 3:49 PM
 */

class VuforiaClient {
    const JSON_CONTENT_TYPE = 'application/json';
    const ACCESS_KEY = 'e9380d90b19fef7aa0e6ad12593c699a29003d34';
    const SECRET_KEY = '276d0916fb5b51db9eb72e77e54318128a8537a8';
    const BASE_URL = 'https://vws.vuforia.com';
    const TARGETS_PATH = '/targets';
    const SUMMARY_PATH = '/summary';
    
    public function addTarget($name,$image,$meta){
        print($name.' '.$image.' '.$meta.'\n');
        $ch = curl_init(self::BASE_URL . self::TARGETS_PATH);
        curl_setopt($ch, CURLOPT_POST, true);
        curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
        curl_setopt($ch, CURLOPT_SSL_VERIFYPEER, false);
        $image_base64 = base64_encode(file_get_contents($image));
        //("http://cache.wine.com/labels/109702l.jpg"));
        //$image_base64 = $image;
        $post_data = array(
            'name' => $name,
            'width' => 150.0,
            'image' => $image_base64,
            'application_metadata' => base64_encode($meta),//$this->createMetadata(),
            'active_flag' => 1
        );
        $body = json_encode($post_data);
        curl_setopt($ch, CURLOPT_HTTPHEADER, $this->getHeaders('POST', self::TARGETS_PATH, self::JSON_CONTENT_TYPE, $body));
        curl_setopt($ch, CURLOPT_POSTFIELDS, $body);
        $response = curl_exec($ch);
        $info = curl_getinfo($ch);
        if ($info['http_code'] !== 201) {
            print 'Failed to add target: ' . $response;
            return 'Failed to add target: ' . $response;
        } else {
            $id = json_decode($response)->target_id;
            print 'Successfully added target: ' . $id . "\n";
            return 'Successfully added target: ' . $id . "\n";
        }
    }
    
    public function deleteTarget($targetID){
        $path = self::TARGETS_PATH . "/" . $targetID;
        $ch = curl_init(self::BASE_URL . $path);
        curl_setopt($ch, CURLOPT_CUSTOMREQUEST, 'DELETE');
        curl_setopt($ch, CURLOPT_HTTPHEADER, $this->getHeaders('DELETE', $path));
        curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
        curl_setopt($ch, CURLOPT_SSL_VERIFYPEER, false);
        $response = curl_exec($ch);
        $info = curl_getinfo($ch);
        if ($info['http_code'] !== 201) {
            print 'Failed to add target: ' . $response;
        } else {
            $id = json_decode($response)->target_id;
            print 'Successfully added target: ' . $id . "\n";
        }
    }
    
    public function deleteAllTargets() {
        $targets = self::getAllTargets();
        print(implode("|",$targets));
        foreach ($targets as $index => $id) {
            self::deleteTarget($id);
        }
    }

    public function getTargetInfo($targetID) {
        $path = self::TARGETS_PATH . "/" . $targetID;
        $ch = curl_init(self::BASE_URL . $path);
        curl_setopt($ch, CURLOPT_HTTPHEADER, $this->getHeaders('GET',$path));
        curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
        curl_setopt($ch, CURLOPT_SSL_VERIFYPEER, false);
        $response = curl_exec($ch);
        $info = curl_getinfo($ch);
        if ($info['http_code'] !== 200) {
            print 'Failed to Get Target Info: ' . '$response = ' . $response . '$info = ' . $info;
        } else {
            $id = json_decode($response)->transaction_id;
            //print 'Successfully grabbed target Info: ' . $id . "|";
            print $response;
            return json_decode($response);
        }
    }
    
    public function getTargetSummary($targetID) {
        $path = self::SUMMARY_PATH . "/" . $targetID;
        $ch = curl_init(self::BASE_URL . $path);
        curl_setopt($ch, CURLOPT_CUSTOMREQUEST, 'GET');
        curl_setopt($ch, CURLOPT_HTTPHEADER, $this->getHeaders('GET', $path));
        curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
        curl_setopt($ch, CURLOPT_SSL_VERIFYPEER, false);
        $response = curl_exec($ch);
        $info = curl_getinfo($ch);
        if ($info['http_code'] !== 200) {
            print 'Failed to Get Target Summary: ' . '$response = ' . $response . '$info = ' . $info;
        } else {
            $id = json_decode($response)->transaction_id;
            //print 'Successfully grabbed target summary: ' . $id . "|";
            print $response;
            return json_decode($response);
        }
    }
    
    public function getAllTargets() {
        $ch = curl_init(self::BASE_URL . self::TARGETS_PATH);
        curl_setopt($ch, CURLOPT_CUSTOMREQUEST, 'GET');
        curl_setopt($ch, CURLOPT_HTTPHEADER, $this->getHeaders('GET'));
        curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
        curl_setopt($ch, CURLOPT_SSL_VERIFYPEER, false);
        $response = curl_exec($ch);
        $info = curl_getinfo($ch);
        if ($info['http_code'] !== 200) {
            print 'Failed to Get All targets: ' . $response;
        } else {
            $id = json_decode($response)->transaction_id;
            //print ($id . "|");
            $results = json_decode($response)->results;
            print(implode("|",$results));
            return $results;
        }
    }
    
    
    private function getHeaders($method, $path = self::TARGETS_PATH, $content_type = '', $body = '') {
        $headers = array();
        $date = new DateTime("now", new DateTimeZone("GMT"));
        $dateString = $date->format("D, d M Y H:i:s") . " GMT";
        $md5 = md5($body, false);
        $string_to_sign = $method . "\n" . $md5 . "\n" . $content_type . "\n" . $dateString . "\n" . $path;
        $signature = $this->hexToBase64(hash_hmac("sha1", $string_to_sign, self::SECRET_KEY));
        $headers[] = 'Authorization: VWS ' . self::ACCESS_KEY . ':' . $signature;
        $headers[] = 'Content-Type: ' . $content_type;
        $headers[] = 'Date: ' . $dateString;
        return $headers;
    }

    public function updateTarget($targetID,$name,$image,$meta)
    {
        $path = self::TARGETS_PATH . "/" . $targetID;
        $ch = curl_init(self::BASE_URL . $path);
        $ch = curl_init(self::BASE_URL . self::TARGETS_PATH);
        curl_setopt($ch, CURLOPT_POST, true);
        curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
        curl_setopt($ch, CURLOPT_SSL_VERIFYPEER, false);
        $image_base64 = base64_encode(file_get_contents($image));
        $post_data = array(
            'name' => $name,
            'width' => 32,
            'image' => $image_base64,
            'active_flag' => 1,
            'application_metadata' => base64_encode($meta)
        );
        $body = json_encode($post_data);
        curl_setopt($ch, CURLOPT_HTTPHEADER, $this->getHeaders('PUT', self::TARGETS_PATH, self::JSON_CONTENT_TYPE, $body));
        curl_setopt($ch, CURLOPT_POSTFIELDS, $body);
        $response = curl_exec($ch);
        $info = curl_getinfo($ch);
        if ($info['http_code'] !== 201) {
            print 'Failed to add target: ' . $response;
            return 'Failed to add target: ' . $response;
        } else {
            $id = json_decode($response)->target_id;
            print 'Successfully added target: ' . $id . "\n";
            return 'Successfully added target: ' . $id . "\n";
        }
    }
    
    private function hexToBase64($hex){
        $return = "";
        foreach(str_split($hex, 2) as $pair){
            $return .= chr(hexdec($pair));
        }
        return base64_encode($return);
    }

    private function createMetadata() {
        $metadata = array(
            'wine_id' => 0,
            'image_url' => "http://cache.wine.com/labels/109702l.jpg",
            'wine_com_url' => "test.com",
            'vintage' => "Test",
            'winery_name' => "Test"
        );
        return base64_encode(json_encode($metadata));
    }
} 