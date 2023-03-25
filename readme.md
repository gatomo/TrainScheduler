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
1. ワークショップからサブスクライブします  

2. コンテンツマネージャーからMODを有効化します  
TrainScheduler 0.x.x

3. [初回のみ]マップを読み込むと以下のフォルダに時刻表が生成されます。 
　フォルダ　[SteamLibraryのインストールフォルダ]\steamapps\common\Cities_Skylines
  ファイル名　TimeTables.xml

4. [初回以外で新しいマップの時刻表を作りたい場合]
　オプションに画面のテキストボックスTimetable fileに新しいファイル名を入力して「Reflect the latest routes in the timetable」ボタンを押下してください。

5. Webで提供するエディタを利用して出発時刻を設定します  
  https://gatomo.github.io/TrainSchedulerEditor/  
  利用方法は有志のかたがこちらの動画で解説してくださっています。  
  　https://youtu.be/RjcQm8UMKLs?t=230  

6. 編集が終わったら時刻表ファイルをダウンロードします  
    このとき、3で示したフォルダに配置してください。
    [SteamLibraryのインストールフォルダ]\steamapps\common\Cities_Skylines

7. ゲームのオプション画面より、Reload Timetableボタンを押下します  
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
    1. End: ModeがIntervalTimeの場合の終電時間を数字4桁で設定します。最大値は2359。使用しない場合は0000を設定してください。  

例）IntervalTimeで06:00始発 10分間隔 終電23:00の場合は以下のように設定する。  
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
最大値は2359です。  


## 路線を追加した / 削除した場合
　オプションに画面のテキストボックスTimetable fileに現在利用している時刻表ファイル名を入力して「Reflect the latest routes in the timetable」ボタンを押下してください。
  ゲーム内で追加削除した路線と停車場が時刻表ファイルに反映されます。

### 削除した場合の注意点
1. 削除した路線、削除した停車場の情報は消失します。ファイルのバックアップを取るか、別名で保存するようにしてください。  
2. 停車場所、番線を変えて停車位置のネットワークIDが変わると注意点1と同様に、その部分の時刻表を消失します。  
  
## 上手く動かない場合  
* 時刻表の確認  
時刻表は「Reload Timetable」直後 または マップロード直後にログに出力されます。上手く設定できていないと感じる場合はログで、意図した出発時刻になっているかご確認ください。  
ログの場所は以下の通りです。  
例）　[SteamLibraryを設定したフォルダ]\steamapps\common\Cities_Skylines\Cities_Data\output_log.txt  
例えば以下のように出力されます。例はLineID=46の1番目の出発時刻  
> === Line[46] has 8 stop(s) ===  
> Stops[0] Departures: 1408 / 1418 / 1428 / 1438 / 1448 / 1458 / 1508 / 1518 / 1528 / 1538 / 1548 / 1558 / 1608 / 1618 / 1628 / 1638 / 1648 / 1703 / 

## 制限事項  
* ゲーム内から時刻表を設定できない（今後対応したい）

## Change Log
### [2.2.4] 2023-03-15
- Station TrackにImproved Transport Managerで設定した駅名をテンプレートと時刻表に出力できるようになりました
- オプションに時刻表のアップデート機能を追加しました。これにより、駅名を更新したり、停車場の追加/削除、路線の追加削除をした際のマージ作業が楽になります。持ち越せる設定はEnabled、UseDefaultTimeTable、Mode、Interval、End、Departuresです。LineIDとStopのIdが同一であれば、設定値を引き継ぎます。
- Timetable.xml以外のファイル名に対応しました。オプション画面より好みの時刻表ファイルを指定、Reloadすることで、好みの時刻表を呼び出すことができます。  
### [2.2.3] 2023-03-15
- Station Trackを利用した際に駅舎Id、次駅Idが0になる場合があり正常に制御されない問題に対処するため、時刻表ファイルに出力されるこれらの値をネットワーク番号に変更しました。これにより全てのマップで時刻表の再設定が必要になります。
### [2.2.2] 2023-03-15
- TimeTables.xmlがない場合でも、開いたマップに応じた設定ファイルを自動で出力するように変更しました。これにより、設定ファイルがない状態で時間をすすめる際に発生していたエラーがなくなります。
- ウェブ上で動作する時刻表エディターをリリースしました。メモ帳等のエディターよりもパターンダイヤの作成が楽になります。https://gatomo.github.io/TrainSchedulerEditor/
### [2.2.1] 2023-03-08
- ソースコード公開に向けた修正
### [2.2.0] 2023-03-08
- 初版公開