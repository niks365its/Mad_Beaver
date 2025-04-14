<?php
header('Content-Type: application/json');


ini_set('display_errors', 1);
ini_set('display_startup_errors', 1);
error_reporting(E_ALL);


$save_folder = __DIR__ . "/data/";
if (!file_exists($save_folder)) {
    mkdir($save_folder, 0777, true);
}

if ($_SERVER['REQUEST_METHOD'] === 'GET') {
    if (isset($_GET['player_id'])) {
        $playerId = preg_replace("/[^a-zA-Z0-9_]/", "", $_GET['player_id']);
        $filename = $save_folder . "progress_" . $playerId . ".json";

        if (file_exists($filename)) {
            echo file_get_contents($filename);
        } else {
            echo json_encode(["status" => "error", "message" => "Прогрес не знайдено"]);
        }
    } else {
        echo json_encode(["status" => "error", "message" => "Не вказано player_id"]);
    }
}

if ($_SERVER['REQUEST_METHOD'] === 'POST') {
    $input = json_decode(file_get_contents("php://input"), true);

    if (isset($input["player_id"]) && isset($input["data"])) {
        $playerId = preg_replace("/[^a-zA-Z0-9_]/", "", $input["player_id"]);
        $filename = $save_folder . "progress_" . $playerId . ".json";
        $data = json_encode($input["data"], JSON_PRETTY_PRINT);

        file_put_contents($filename, $data);
        echo json_encode(["status" => "success", "message" => "Прогрес збережено"]);
    } else {
        echo json_encode(["status" => "error", "message" => "Некоректні дані"]);
    }
}
