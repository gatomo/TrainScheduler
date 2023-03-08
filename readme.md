# Timetable MOD
このMODを使用することで、Cities: Skylinesの__地上鉄道__向けに発車時刻を設定することができます。

## 必須MOD
Harmony  
https://steamcommunity.com/workshop/filedetails/?id=2040656402  
UUI  
https://steamcommunity.com/workshop/filedetails/?id=2255219025  

## 推奨MOD
Real Time  
https://steamcommunity.com/sharedfiles/filedetails/?id=1420955187&searchtext=RealTime  
※Ultimate EyeCandyが非推奨になったため、Real Timeを利用することで時刻を確認することが可能になります

## 競合MOD
Improved Public Transport 2

## 使い方
1. zipを以下のフォルダに展開します  
　C:\Users\\[ユーザー名]\AppData\Local\Colossal Order\Cities_Skylines\Addons\Mods\TrainScheduler 
展開後のイメージ  
　C:\Users\\[ユーザー名]\AppData\Local\Colossal Order\Cities_Skylines\Addons\Mods\TrainScheduler\TrainScheduler.dll  
　C:\Users\\[ユーザー名]\AppData\Local\Colossal Order\Cities_Skylines\Addons\Mods\TrainScheduler\CitiesHarmony.API.dll  
　C:\Users\\[ユーザー名]\AppData\Local\Colossal Order\Cities_Skylines\Addons\Mods\TrainScheduler\TimeTables.xml  
　C:\Users\\[ユーザー名]\AppData\Local\Colossal Order\Cities_Skylines\Addons\Mods\TrainScheduler\readme.md  
　C:\Users\\[ユーザー名]\AppData\Local\Colossal Order\Cities_Skylines\Addons\Mods\TrainScheduler\DepartureTimeEditor.exe  
　C:\Users\\[ユーザー名]\AppData\Local\Colossal Order\Cities_Skylines\Addons\Mods\TrainScheduler\DepartureTimeEditor.pdb  
　C:\Users\\[ユーザー名]\AppData\Local\Colossal Order\Cities_Skylines\Addons\Mods\TrainScheduler\Resources\mod-icon.png  

2. コンテンツマネージャーからMODを有効化します  
TrainScheduler 0.x.x

3. 時刻表を設定したいマップを読み込みます  

4. オプションより TrainSchedulerを選択し、Create Templateを押下します  
以下の設定ファイルが作成されます。
ファイルの場所  
　C:\Users\\[ユーザー名]\AppData\Local\Colossal Order\Cities_Skylines\Addons\Mods\TrainScheduler\TimeTable_Template.xml  
マップ内の路線IDとそれぞれの駅のインデックスが出力されているはずです。  
XML内の構造はこのような形です。  
　TimeTableData  
　┗Lines  
　　┗Line　←路線ID、路線名、駅数、交通タイプ、モード  
　　　┗Stops  
　　　　┗Stop　←停車番号（０からスタート）、駅舎ID、駅名、次の駅舎ID、次の駅名、モード、インターバル  
　　　　　┗Departures  
　　　　　　┗Departure　←出発時刻  

5. 出発時刻を設定します  
以下のようにHHmm形式で入力してください。その他詳細は 変更できるオプション をご確認ください。  
　\<Departure\>0002\</Departure\>　←00時02分  
複数設定する場合はDepartureタグを追加し、以下のようにしてください。  
　\<Departure\>0002\</Departure\>  
　\<Departure\>2352\</Departure\>

7. TimeTables_Template.xmlをTimeTables.xmlにリネームします  
時刻表MODはTimeTables.xmlのみを時刻表として認識します。  

8. ゲームのオプション画面より、Reload Timetableボタンを押下します  
これにて時刻表の設定は完了です。ゲームをお楽しみください。

## 変更できるオプション
以下に示すオプションを変更できます。記載していない設定値はMODの動作に必要なものであるため、変更しないでください。

1. Line: 路線全体に影響するオプションです。路線ごとと駅ごとで同じ設定項目があった場合、路線の設定が優先です。例えば路線全体で10分間隔で運行するためのオプションがオンの場合は、駅ごとに個別の発車時刻を設定しても10分間隔の運行が優先されます。
    1. Enabled: 時刻表MODによる制御の有効・無効を切り替えます。
        1. true: 有効
        1. false: 無効。無効にした場合、この路線全体がバニラと同じ挙動をします。
    1. Mode: 時刻設定の単位を切り替えます。
        1. Separately: 駅ごとに発車時刻を設定します。
        1. FirstToAll: 1駅目の発車時刻を路線内の全駅に展開します。※正しく動作しないため使用しないでください。
    1. UseDefaultTimeTable: 10分間隔で運行するためのオプションです。時刻表を設定することが面倒になったときにご利用ください。
        1. true: 有効。毎時00/10/20/30/40/50分に電車が発車するようになります。
        1. false: 無効。個別に設定した発車時刻を使用します。
1. Stop: 駅ごと（正確には路線の停車場所ごと）に影響するオプションです。路線A、路線Bが駅Sの同じ番線に停車するとしても、設定が互いに干渉することはありません。
    1. Enabled: 時刻表MODによる制御の有効・無効を切り替えます。
        1. true: 有効
        1. false: 無効。無効にした場合、この駅の発車時刻はバニラと同じ挙動をします。
    1. UseDefaultTimeTable: 10分間隔で運行するためのオプションです。時刻表を設定することが面倒になったときにご利用ください。
        1. true: 有効。毎時00/10/20/30/40/50分に電車が発車するようになります。
        1. false: 無効。個別に設定した発車時刻を使用します。
    1. Mode: 発車時刻の設定を間隔または個別指定を選択します
        1. Indivisually: 全ての発車時刻を個別に指定します。手間はかかりますが、細かい調整が可能。  
        1. IntervalTime: 間隔(分)で発車時刻を指定します。後続のIntervalオプションとEndオプションの設定必須。 
    1. Interval: 発車間隔を正の整数で指定します。Stopオプション内のModeがIntervalTimeの場合のみ有効です。ModeがIndivisuallyの場合でも整数を設定しておいてください。
    1. End: ModeがIntervalTimeの場合の終電時間を数字4桁で設定します。使用しない場合は0000を設定してください。

例）IntervalTimeで06:00始発 10分間隔 終電23:00の場合は以下のように設定する  
        \<Stop StopNumber="0" Mode="IntervalTime" Interval="10" End="2300">  
          \<Departures>  
            \<Departure>0600\</Departure>

例）Inidivisuallyで個別に出発時刻を設定する
        \<Stop StopNumber="0" Mode="Indivisually" Interval="-1" End="0000">  
          \<Departures>  
            \<Departure>0000\</Departure>  
            \<Departure>0010\</Departure>  
            \<Departure>0020\</Departure>  
            \<Departure>0030\</Departure>  

## 路線を追加した / 削除した場合
使い方 5～8を再度実行しなおしてください。既存路線は設定済みの時刻表データをコピーすると楽になります。  

## 

## 上手く動かない場合  
* 旧TimeTable Modをインストールしている場合は、削除するか無効状態にしてください
* 時刻表の確認  
時刻表は「Reload Timetable」直後 または マップロード直後にログに出力されます。上手く設定できていないと感じる場合はログで、意図した出発時刻になっているかご確認ください。  
ログの場所は以下の通りです。  
例）　[SteamLibraryを設定したフォルダ]\steamapps\common\Cities_Skylines\Cities_Data\output_log.txt  
例えば以下のように出力されます。例はLineID=46の1番目の出発時刻  
> === Line[46] has 8 stop(s) ===  
> Stops[0] Departures: 1408 / 1418 / 1428 / 1438 / 1448 / 1458 / 1508 / 1518 / 1528 / 1538 / 1548 / 1558 / 1608 / 1618 / 1628 / 1638 / 1648 / 1703 / 

## 注意  
* 本MODをWorkshopに公開しないでください
* 不具合報告などはDiscordにて

## 制限事項  
* ゲーム内から時刻表を設定できない（今後対応したい）