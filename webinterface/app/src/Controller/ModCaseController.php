<?php


namespace App\Controller;

use App\API\FilesAPI;
use App\Helpers\BasicData;
use App\API\CommentsAPI;
use App\API\DiscordAPI;
use App\API\GuildConfigAPI;
use App\API\ModCaseAPI;
use App\Config\Config;
use App\Helpers\Helpers;
use Exception;
use Symfony\Bundle\FrameworkBundle\Controller\AbstractController;
use Symfony\Component\HttpClient\HttpClient;
use Symfony\Component\Routing\Annotation\Route;

class ModCaseController extends AbstractController
{

    /**
     * @Route("/modcases/{guildid}/{id}", requirements={"guildid"="[0-9]{18}", "id"="\d*"})
     */
    public function showCase($guildid, $id)
    {
        if (!isset($_COOKIE["masz_access_token"])) {
            return $this->render('index.html.twig');
        }

        $basicData = new BasicData($_COOKIE);
        $basicData->currentGuild = $guildid;
        if (is_null($basicData->loggedInUser)) {
            $basicData->errors[] = 'You have been logged out.';
            return $this->render('index.html.twig', [
                'basic_data' => $basicData
            ]);
        }

        $modCase = ModCaseAPI::Select($_COOKIE, $guildid, $id);
        if (!$modCase->success || is_null($modCase->body) || $modCase->statuscode !== 200) {
            $basicData->errors[] = 'Failed to load modcase. API: ' . $modCase->toString();
            return $this->render('modcase/view.html.twig', [
                'basic_data' => $basicData
            ]);
        }
        $modCase = $modCase->body;

        $guild = DiscordAPI::GetGuild($_COOKIE, $guildid)->body;
        if (is_null($guild)) {
            $basicData->errors[] = 'Failed to load detailed info about guild';
        }

        $moderator = DiscordAPI::GetUser($_COOKIE, $modCase['modId'])->body;
        if (is_null($moderator)) {
            $basicData->errors[] = 'Failed to load detailed info about moderator';
        }

        $lastModerator = DiscordAPI::GetUser($_COOKIE, $modCase['lastEditedByModId'])->body;
        if (is_null($lastModerator)) {
            $basicData->errors[] = 'Failed to load detailed info about last moderator';
        }

        $caseUser = DiscordAPI::GetUser($_COOKIE, $modCase['userId'])->body;
        if (is_null($caseUser)) {
            $basicData->errors[] = 'Failed to load detailed user info';
        }

        $files = FilesAPI::SelectAll($_COOKIE, $guildid, $modCase['caseId'])->body;
        if (is_null($files)) {
            $basicData->errors[] = 'Failed to load uploaded files.';
        }

        $newComments = [];  // comments with discord user object merged
        $fetchedUser = [];
        foreach ($modCase['comments'] as $comment) {
            if (!array_key_exists($comment['userId'], $fetchedUser)) {
                $comment['discordUser'] = DiscordAPI::GetUser($_COOKIE, $comment['userId'])->body;
            } else {
                $comment['discordUser'] = $fetchedUser[$comment['userId']];
            }
            $newComments[] = $comment;
        }
        $modCase['comments'] = $newComments;

        $basicData->tabTitle = 'MASZ: #'.$modCase['caseId'].': '.$modCase['title'];
        return $this->render('modcase/view.html.twig', [
            'basic_data' => $basicData,
            'modcase' => $modCase,
            'guild' => $guild,
            'moderator' => $moderator,
            'lastModerator' => $lastModerator,
            'files' => $files,
            'user' => $caseUser
        ]);
    }
}
