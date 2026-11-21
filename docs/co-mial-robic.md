# Co miał robić minutnik — specyfikacja po fakcie

## Co spisujesz dzisiaj

Specyfikacja spisana po fakcie (inżynieria wsteczna) z działającej aplikacji minutnika, wyłącznie na podstawie obserwacji jej uruchomionego interfejsu i działania, sporządzona z pamięci i bez zaglądania do lekcji o minutniku. Dokument rejestruje stan zastany aplikacji, jej rzeczywiste możliwości, braki funkcjonalne, obserwowalne zachowania weryfikacyjne oraz kwestie wymagające doprecyzowania.

---

## 1. Co robi

1. Odmierza czas pracy (domyślnie 25 minut) oraz czas przerwy (domyślnie 5 minut) w trybie odliczania wstecznego (od zadanej liczby minut i sekund do zera).
2. Wyświetla aktualny pozostały czas w czytelnym formacie cyfrowym `mm:ss` (dwie cyfry minut i dwie cyfry sekund) na dużym wyświetlaczu w centralnej części okna.
3. Wizualizuje postęp upływającego czasu za pomocą poziomego paska postępu (`ProgressBar`), który dynamicznie wypełnia się proporcjonalnie do upływu bieżącej sesji.
4. Umożliwia uruchomienie odliczania oraz jego wstrzymanie w dowolnym momencie za pomocą jednego przełączanego przycisku „Start” / „Pauza”.
5. Umożliwia swobodną konfigurację czasu pracy (od 1 do 180 minut) oraz czasu przerwy (od 1 do 120 minut) za pomocą kontrolek numerycznych (`NumericUpDown`) bezpośrednio w oknie aplikacji.
6. Automatycznie przełącza się z trybu pracy do trybu przerwy, gdy licznik pracy osiągnie `00:00`, i samoczynnie kontynuuje odliczanie przerwy bez konieczności klikania czegokolwiek przez użytkownika.
7. Zlicza skończone pełne cykle i wyświetla ich sumaryczną liczbę na dedykowanym kafelku „CYKLE / ukończone”, znajdującym się tuż obok wyświetlacza czasu.
8. Rejestruje historię ukończonych cykli na liście (`ListBox`), dopisując po pomyślnym zakończeniu każdej przerwy wpis ze znacznikiem czasu i numerem ukończonego cyklu (w formacie `HH:mm — cykl X`).
9. Wyraźnie sygnalizuje aktualny stan i fazę minutnika: plakietka trybu („PRACA” / „PRZERWA”) ze zmieniającym się kolorem obramowania i tła (ciepłe odcienie pomarańczowe dla pracy, zielone dla przerwy) oraz opisowy komunikat statusu na dole okna (np. „Gotowy do rozpoczęcia pracy”, „Trwa praca (skupienie)...”, „Wstrzymano”).
10. Umożliwia zresetowanie stanu minutnika za pomocą przycisku „Reset”, co zatrzymuje odliczanie, przywraca zadany czas pracy, przełącza tryb na pracę oraz zeruje licznik ukończonych cykli.

---

## 2. Czego nie robi

1. Nie odtwarza słyszalnych sygnałów dźwiękowych ani alarmów akustycznych po zakończeniu fazy pracy lub przerwy (wywołanie `Console.Beep()` w kodzie w typowym środowisku graficznym Windows/Linux nie generuje słyszalnego dźwięku w interfejsie).
2. Nie wyświetla powiadomień systemowych systemu operacyjnego (dymków toast / powiadomień pulpitu), gdy okno programu nie jest na wierzchu.
3. Nie zapisuje historii cykli ani zmodyfikowanych ustawień czasu do trwałego pliku na dysku (np. JSON, XML) ani bazy danych — po zamknięciu programu historia i konfiguracja bezpowrotnie przepadają.
4. Nie pozwala na ręczne czyszczenie, edytowanie ani usuwanie pojedynczych wpisów z widocznej listy historii.
5. Nie czyści listy historii ukończonych cykli po naciśnięciu przycisku „Reset” (zeruje jedynie licznik numeryczny, wpisy tekstowe na liście pozostają nienaruszone).
6. Nie pozwala na wprowadzanie ułamkowych wartości minut (wartości dziesiętne z kropką lub przecinkiem są zaokrąglane lub odrzucane przez kontrolkę numeryczną).
7. Nie minimalizuje się do zasobnika systemowego (system tray) ani nie działa w tle jako usługa.
8. Nie obsługuje automatycznych długich przerw (tzw. Long Break) po ukończeniu serii czterech cykli Pomodoro.

---

## 3. Pięć zdań „Kiedy …, to …”

1. Kiedy kliknę przycisk „Start”, to minutnik natychmiast rozpoczyna odliczanie czasu w dół co sekundę, tekst przycisku zmienia się na „Pauza”, a status informuje o trwaniu pracy w skupieniu.
2. Kiedy w trakcie trwania odliczania kliknę przycisk „Pauza”, to zegar zatrzymuje się na bieżącej wartości sekund, tekst przycisku zmienia się z powrotem na „Start”, a status przyjmuje treść „Wstrzymano”.
3. Kiedy czas pracy dobiegnie do 00:00, to tryb automatycznie zmienia się na przerwę, kolorystyka akcentów i plakietki przełącza się z pomarańczowej na zieloną, a minutnik od razu samoczynnie rozpoczyna odliczanie czasu przerwy.
4. Kiedy czas przerwy dobiegnie do 00:00, to licznik ukończonych cykli zwiększa się o 1, do listy dopisywany jest nowy wpis z bieżącą godziną i numerem ukończonego cyklu, a minutnik przełącza się z powrotem w tryb pracy i zatrzymuje w oczekiwaniu na ponowne kliknięcie Start.
5. Kiedy w trakcie odliczania kliknę przycisk „Reset”, to zegar zatrzymuje się, wyświetlany czas powraca do skonfigurowanej długości sesji pracy (25:00), tryb przełącza się na pracę, a licznik ukończonych cykli zostaje wyzerowany do 0.

---

## 4. Do ustalenia

1. **Zachowanie licznika cykli przy wciśnięciu „Reset” w środku przerwy**: czy wciśnięcie resetu podczas odpoczynku powinno bezpowrotnie kasować licznik już pomyślnie przepracowanych sesji skupienia, czy reset powinien dotyczyć wyłącznie bieżącej przerwy.
2. **Spójność resetu z listą historii**: dlaczego przycisk „Reset” zeruje licznik cykli na kafelku, lecz pozostawia nienaruszoną listę historii w `ListBox` — czy pełny reset powinien czyścić również listę zdarzeń.
3. **Powiadomienia dźwiękowe i systemowe**: czy aplikacja powinna wydawać głośny sygnał dźwiękowy lub wysyłać powiadomienie pulpitu po zakończeniu odliczania, aby użytkownik nie musiał stale kontrolować okna wzrokiem.
4. **Wpływ zmiany długości na bieżącą sesję**: czy zmiana wartości w polach `NumericUpDown` podczas aktywnego odliczania powinna dynamicznie korygować pozostały czas bieżącej sesji, czy kontrolki te powinny być blokowane do momentu zakończenia lub zresetowania timera.
5. **Długie przerwy (Long Break)**: czy aplikacja powinna oferować wydłużoną przerwę (np. 15–30 minut) po ukończeniu każdych czterech pełnych cykli roboczych.


---

## Porównanie z siedmioma zasadami z lekcji

Po sporządzeniu powyższej specyfikacji z pamięci, skonfrontowano ją z siedmioma zasadami działania minutnika z lekcji:

1. **Odmierzanie 25 minut pracy** — odtworzona (domyślny czas odliczania sesji skupienia).
2. **Odmierzanie 5 minut przerwy** — odtworzona (domyślny czas trwania sesji regeneracji).
3. **Liczenie skończonych cykli** — odtworzona (zliczanie ukończonych cykli po zakończeniu przerwy).
4. **Obie długości mają być do zmiany w oknie** — odtworzona (kontrolki `NumericUpDown` dla pracy i przerwy).
5. **Jeden przycisk start/pauza, drugi reset** — odtworzona (przycisk przełączający Start/Pauza oraz osobny Reset).
6. **Czas widoczny jako mm:ss, licznik cykli widoczny obok** — odtworzona (duży wyświetlacz `00:00` oraz sąsiadujący kafelek z cyframi cykli).
7. **Po skończonej pracy przerwa ma ruszyć sama** — odtworzona (automatyczne przejście z trybu pracy do przerwy bez zatrzymywania zegara).

### Ile udało się odtworzyć
Z pamięci udało się odtworzyć **wszystkie 7 z 7 zasad (100%)**. Każda z tych zasad wynika bezpośrednio z układu kontrolek graficznych oraz obserwowalnego zachowania działającej aplikacji.

### Co program robi w sytuacji, o której żadna z nich nie mówiła
Żadna z siedmiu zasad z lekcji nie precyzowała, **co ma się stać z licznikiem cykli, kiedy naciśniesz reset w środku przerwy** (ani ogólnie: jaki jest zakres działania przycisku Reset względem licznika cykli).

W kodzie programu (`minutnik/MainWindow.axaml.cs`, metoda `OnResetClick`) w tej sytuacji program:
- zatrzymuje bieżący timer (`_timer.Stop()`),
- bezwarunkowo przełącza stan sesji na pracę (`_isWorkSession = true`),
- ustawia licznik czasu na pełny czas pracy (`_remainingSeconds = _workDurationMinutes * 60`),
- **całkowicie zeruje licznik ukończonych cykli do zera (`_completedCycles = 0`)**,
- jednocześnie pozostawia dotychczasową listę historii (`_history` / `HistoryList`) bez zmian.

Zasady z pokazu mówiły jedynie: *„Jeden przycisk start/pauza, drugi reset”*, nie definiując, czy reset ma dotyczyć jedynie bieżącej fazy odliczania, czy kasować cały uzyskany bilans cykli roboczych.
