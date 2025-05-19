using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public class WordEntry
    {
        public int id;
        public string en;
        public string ru;
        public string tr;
    }

    [System.Serializable]
    public class WordEntryList
    {
        public List<WordEntry> items;
    }

    public List<GameObject> englishButtons;           // Кнопки с английскими словами
    public List<GameObject> russianButtons;           // Кнопки с русскими словами
    public TextAsset jsonFile;                        // JSON-файл со словами

    public TMP_Text totalWordsText;                   // UI: всего слов
    public TMP_Text completedWordsText;               // UI: завершено

    private List<WordEntry> wordList;                 // Загруженный список слов
    private int currentIndex = 0;                     // Индекс текущей порции слов

    private WordButtonUI selectedWord1 = null;        // Первая выбранная кнопка
    private List<WordButtonUI> incorrectButtons = new List<WordButtonUI>(); // Неверные кнопки для сброса

    void Start()
    {
        LoadWords();             // Загрузка слов
        AssignWordsToButtons(); // Первичное отображение
    }

    void LoadWords()
    {
        if (jsonFile == null)
        {
            Debug.LogError("Не присвоен файл JSON");
            return;
        }

        WordEntryList data = JsonUtility.FromJson<WordEntryList>(jsonFile.text);
        wordList = data.items;

        if (wordList == null || wordList.Count == 0)
        {
            Debug.LogError("Не удалось загрузить слова из JSON.");
        }

        UpdateStatsUI(); // Обновление статистики
    }

    public void AssignWordsToButtons()
    {
        if (currentIndex >= wordList.Count)
        {
            Debug.Log("Слова закончились");
            return;
        }

        int count = Mathf.Min(5, wordList.Count - currentIndex);
        List<WordEntry> selected = wordList.GetRange(currentIndex, count);

        Shuffle(selected); // Перемешиваем английские
        List<WordEntry> shuffledRussian = new List<WordEntry>(selected);
        Shuffle(shuffledRussian); // Перемешиваем русские

        for (int i = 0; i < count; i++)
        {
            var entryEn = selected[i];
            var enBtn = englishButtons[i];
            var enComp = enBtn.GetComponent<WordButtonUI>();
            enComp.ResetState();
            enBtn.GetComponentInChildren<TMP_Text>().text = entryEn.tr + "\n" + entryEn.en;
            enComp.word = entryEn.en;
            enComp.pairId = entryEn.id;
            enComp.SetManager(this);
            enComp.MakeVisible(true);

            var entryRu = shuffledRussian[i];
            var ruBtn = russianButtons[i];
            var ruComp = ruBtn.GetComponent<WordButtonUI>();
            ruComp.ResetState();
            ruBtn.GetComponentInChildren<TMP_Text>().text = entryRu.ru;
            ruComp.word = entryRu.ru;
            ruComp.pairId = entryRu.id;
            ruComp.SetManager(this);
            ruComp.MakeVisible(true);
        }

        // Прячем неиспользуемые кнопки
        for (int i = count; i < englishButtons.Count; i++)
        {
            englishButtons[i].GetComponent<WordButtonUI>().MakeVisible(false);
            russianButtons[i].GetComponent<WordButtonUI>().MakeVisible(false);
        }

        selectedWord1 = null;
        incorrectButtons.Clear();

        EventSystem.current.SetSelectedGameObject(null);
        UpdateStatsUI();
    }

    public void OnWordButtonClicked(WordButtonUI button)
    {
        if (incorrectButtons.Count > 0)
        {
            foreach (var b in incorrectButtons)
                b.ResetState();
            incorrectButtons.Clear();
        }

        if (selectedWord1 == null)
        {
            selectedWord1 = button;
            button.Highlight(true);
            return;
        }

        // Если нажата кнопка из той же колонки
        if (IsSameColumn(selectedWord1.gameObject, button.gameObject))
        {
            if (selectedWord1 == button)
            {
                // Повторный клик по той же кнопке → отмена выбора
                selectedWord1.Highlight(false);
                selectedWord1 = null;
            }
            else
            {
                // Отмена предыдущей и выделение новой в той же колонке
                selectedWord1.Highlight(false);
                selectedWord1 = button;
                button.Highlight(true);
            }
            return;
        }

        // Кнопки из разных колонок → сравнение
        WordButtonUI selectedWord2 = button;
        button.Highlight(true);

        if (selectedWord1.pairId == selectedWord2.pairId)
        {
            selectedWord1.MakeVisible(false);
            selectedWord2.MakeVisible(false);
            selectedWord1 = null;
            CheckRoundEnd();
        }
        else
        {
            selectedWord1.MarkIncorrect();
            selectedWord2.MarkIncorrect();
            incorrectButtons.Add(selectedWord1);
            incorrectButtons.Add(selectedWord2);
            selectedWord1 = null;
        }

        EventSystem.current.SetSelectedGameObject(null);
    }

    bool IsSameColumn(GameObject a, GameObject b)
    {
        return (englishButtons.Contains(a) && englishButtons.Contains(b)) ||
               (russianButtons.Contains(a) && russianButtons.Contains(b));
    }

    void CheckRoundEnd()
    {
        bool allInvisible = true;

        for (int i = 0; i < englishButtons.Count; i++)
        {
            var enComp = englishButtons[i].GetComponent<WordButtonUI>();
            var ruComp = russianButtons[i].GetComponent<WordButtonUI>();
            if (enComp.IsVisible()) allInvisible = false;
            if (ruComp.IsVisible()) allInvisible = false;
        }

        if (allInvisible)
        {
            currentIndex += 5;
            AssignWordsToButtons();
        }
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int r = Random.Range(i, list.Count);
            T tmp = list[i];
            list[i] = list[r];
            list[r] = tmp;
        }
    }

    void UpdateStatsUI()
    {
        if (totalWordsText != null)
            totalWordsText.text = $"Всего слов: {wordList.Count}";

        if (completedWordsText != null)
            completedWordsText.text = $"Пройдено: {currentIndex}";
    }
}

