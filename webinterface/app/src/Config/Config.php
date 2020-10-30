<?php

namespace App\Config;

use Exception;
use Symfony\Component\HttpClient\HttpClient;

class Config
{
    public static function getAPIBaseURL() {
        // return 'http://127.0.0.1:5565';  // for local development
        return 'http://masz_backend:80';  // for docker deployment
    }
    public static function getBaseURL() {
        // return 'http://127.0.0.1:5565';  // for local development
        return 'http://masz_nginx:80';  // for docker deployment
    }
}