namespace SimpleFunctions
{
    #region шифры
    // https://habr.com/ru/post/271257/
    struct TypeAlphabet
    {
        public bool Numbers,
                    Simvols,
                    UpperRussian,
                    UpperEnglish,
                    LowerRussian,
                    LowerEnglish,
                    All;
    }
    /// <summary>
    /// шифры
    /// </summary>
    abstract class Ciphers
    {
        protected string alphabet;
        /// <summary>
        /// конструктор создания алфавита для шифра, который использует все символы
        /// </summary>
        public Ciphers()
        {
            alphabet = "";
            for (int i = 0; i < char.MaxValue; i++)
                alphabet += (char)i;//перечисление всех символов
        }
        /// <summary>
        /// конструктор создания алфавита для шифра
        /// </summary>
        /// <param name="type">шаблон алфавита</param>
        public Ciphers(TypeAlphabet type)
        {
            alphabet = "";
            if (type.Numbers || type.All)
                alphabet += "0123456789";
            if (type.Simvols || type.All)
                alphabet += "/|\"\\;'?:=.,&!@#$%^*()_+- ";
            if (type.UpperRussian || type.All)
                alphabet += "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
            if (type.UpperEnglish || type.All)
                alphabet += "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            if (type.LowerRussian || type.All)
                alphabet += "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";
            if (type.LowerEnglish || type.All)
                alphabet += "abcdefghijklmnopqrstuvwxyz";
        }
        /// <summary>
        /// конструктор создания алфавита для шифра
        /// </summary>
        /// <param name="alphabet">собственный алфавит</param>
        public Ciphers(string alphabet)
        {
            this.alphabet = alphabet;
        }

    }
    /// <summary>
    /// Шифр простой замены
    /// </summary>
    class SimpleReplacementCipher/*Шифр простой замены*/ : Ciphers
    {
        public SimpleReplacementCipher() : base()
        {
        }
        public SimpleReplacementCipher(TypeAlphabet type) : base(type)
        {
        }
        public SimpleReplacementCipher(string alphabet) : base(alphabet)
        {
        }
        /// <summary>
        /// шифрование текста
        /// </summary>
        /// <param name="text">текст</param>
        /// <param name="key">ключ</param>
        /// <returns>зашифрованый текст</returns>
        /// <exception cref="Exception">размер ключа должен быть равен размеру алфавита</exception>
        public string Encrypt(string text, string key)//шифрование текста
        {
            string newText = "";
            if (key.Length != alphabet.Length)
                throw new Exception("размер ключа должен быть равен размеру алфавита");
            for (int i = 0; i < text.Length; i++)
            {
                int idChar = alphabet.IndexOf(text[i]);
                if (idChar < 0)
                    newText += text[i];
                else
                    newText += key[idChar];
            }
            return newText;
        }
        /// <summary>
        /// дешифрование текста
        /// </summary>
        /// <param name="text">текст</param>
        /// <param name="key">ключ</param>
        /// <returns>расшифрованый текст</returns>
        /// <exception cref="Exception">размер ключа должен быть равен размеру алфавита</exception>
        public string Decrypt(string text, string key)//дешифрование текста
        {
            string newText = "";
            if (key.Length != alphabet.Length)
                throw new Exception("размер ключа должен быть равен размеру алфавита");

            for (int i = 0; i < text.Length; i++)
            {
                int idChar = key.IndexOf(text[i]);
                if (idChar < 0)
                    newText += text[i];
                else
                    newText += alphabet[idChar];
            }
            return newText;
        }
        /// <summary>
        /// создать ключ автоматически
        /// </summary>
        /// <returns>ключ</returns>
        public string AutoGenerateKey()
        {
            string key = "";
            List<char> tempAlphabet = new List<char>();
            tempAlphabet.AddRange(alphabet);
            for (int i = 0; i < alphabet.Length; i++)
            {
                Random r = new Random();
                int idChar = r.Next(0, tempAlphabet.Count);
                key += tempAlphabet[idChar];
                tempAlphabet.RemoveAt(idChar);
            }
            return key;
        }
    }
    /// <summary>
    /// Аффиный шифр
    /// </summary>
    class AffineCipher/*Аффиный шифр*/ : Ciphers
    {
        public AffineCipher() : base()
        {
        }
        public AffineCipher(TypeAlphabet type) : base(type)
        {
        }
        public AffineCipher(string alphabet) : base(alphabet)
        {
        }
        /// <summary>
        /// шифрование текста
        /// </summary>
        /// <param name="text">текст</param>
        /// <param name="key1">ключ, который входит в допусимые мультипликативные пары</param>
        /// <param name="key2">ключ (смещение)</param>
        /// <returns>зашифрованый текст</returns>
        public string Encrypt(string text, int key1, int key2)//шифрование текста
        {
            return CodeEncode(text, key1, key2, true);
        }
        /// <summary>
        /// дешифрование текста
        /// </summary>
        /// <param name="text">текст</param>
        /// <param name="key1">ключ, который входит в допусимые мультипликативные пары</param>
        /// <param name="key2">ключ (смещение)</param>
        /// <returns>расшифрованый текст</returns>
        public string Decrypt(string text, int key1, int key2)//дешифрование текста
        {
            return CodeEncode(text, key1, key2, false);
        }
        private string CodeEncode(string text, int key1, int key2, bool encrypt)
        {
            int multy_key1;
            if (!IsValidMultiplicativePairs(key1, out multy_key1))
                throw new Exception("ключ не входит в допусимые мультипликативные пары"); ;
            string newText = "";
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                int index = alphabet.IndexOf(c);
                if (index < 0)
                {
                    //если символ не найден, то добавляем его в неизменном виде
                    newText += c.ToString();
                }
                else
                {
                    int codeIndex;
                    if (encrypt)
                        codeIndex = (index * key1 + key2) % alphabet.Length;
                    else
                        codeIndex = ((index - key2) * multy_key1) % alphabet.Length;
                    if (codeIndex < 0)
                        codeIndex = alphabet.Length + codeIndex;
                    newText += alphabet[codeIndex];
                }
            }
            return newText;
        }
        /// <summary>
        /// выводит допусимые мультипликативные пары
        /// </summary>
        /// <returns>мультипликативные пары</returns>
        public int[,] ValidMultiplicativePairs()
        {
            int[,] multiplicativeKeys;
            List<int> key1 = new List<int>(),
                      key2 = new List<int>();
            for (int i = 0; i < alphabet.Length; i++)
                for (int j = 0; j < alphabet.Length; j++)
                    if ((i * j) % alphabet.Length == 1)
                    {
                        key1.Add(i);
                        key2.Add(j);
                    }
            multiplicativeKeys = new int[key1.Count, 2];
            for (int i = 0; i < key1.Count; i++)
            {
                multiplicativeKeys[i, 0] = key1[i];
                multiplicativeKeys[i, 1] = key2[i];
            }
            return multiplicativeKeys;
        }
        /// <summary>
        /// проверка на допусимые мультипликативные пары
        /// </summary>
        /// <param name="key1">первый ключ пары</param>
        /// <param name="key2">второй ключ пары</param>
        /// <returns>допусимые ли мультипликативные пары</returns>
        public bool IsValidMultiplicativePairs(int key1, out int key2)
        {
            key2 = -1;
            for (int i = 0; i < alphabet.Length; i++)
                for (int j = 0; j < alphabet.Length; j++)
                    if ((i * j) % alphabet.Length == 1 && key1 == i)
                    {
                        key2 = j;
                        return true;
                    }
            return false;
        }
    }
    /// <summary>
    /// Шифр Плейфера
    /// </summary>
    class PlayfairCipher/*Шифр Плейфера*/ : Ciphers
    {
        public PlayfairCipher() : base()
        {
        }
        public PlayfairCipher(TypeAlphabet type) : base(type)
        {
        }
        public PlayfairCipher(string alphabet) : base(alphabet)
        {
        }
        /// <summary>
        /// шифрование текста
        /// </summary>
        /// <param name="text">текст</param>
        /// <param name="keyMatrix">ключ-матрица</param>
        /// <returns>зашифрованый текст</returns>
        public string Encrypt(string text, char[,] keyMatrix)//шифрование текста
        => CodeEncode(text, keyMatrix, true);
        /// <summary>
        /// дешифрование текста
        /// </summary>
        /// <param name="text">текст</param>
        /// <param name="keyMatrix">ключ-матрица</param>
        /// <returns>расшифрованый текст</returns>
        public string Decrypt(string text, char[,] keyMatrix)//дешифрование текста
        => CodeEncode(text, keyMatrix, false);
        private string CodeEncode(string text, char[,] keyMatrix, bool encrypt)
        {
            if (keyMatrix.GetLength(0) != keyMatrix.GetLength(1))
                throw new Exception("ключ-матрица должна быть квадратной");

            string newText = "";
            for (int i = 1; i < text.Length; i += 2)
            {
                if (CharInKeyExist(text[i - 1], keyMatrix) && CharInKeyExist(text[i], keyMatrix))
                {
                    (char c1, char c2) = BigramConversion(text[i - 1], text[i], keyMatrix, encrypt);
                    newText += c1;
                    newText += c2;
                }
                else
                {
                    newText += text[i - 1];
                    newText += text[i];
                }
            }
            if (text.Length % 2 != 0)
                newText += text[text.Length - 1];
            return newText;
        }
        (char, char) BigramConversion(char c1, char c2, char[,] keyMatrix, bool encrypt)
        {
            (int x1, int y1) = PositionCharInKey(c1, keyMatrix);
            (int x2, int y2) = PositionCharInKey(c2, keyMatrix);
            int newX1 = x1, newX2 = x2,
                newY1 = y1, newY2 = y2;

            if (x1 == x2)
            {
                if (encrypt)
                {
                    newX1++;
                    newX2++;
                }
                else
                {
                    newX1--;
                    newX2--;
                }
                if (newX1 == keyMatrix.GetLength(0))
                {
                    newX1 = 0;
                    newX2 = 0;
                }
                if (newX1 == -1)
                {
                    newX1 = keyMatrix.GetLength(0) - 1;
                    newX2 = keyMatrix.GetLength(0) - 1;
                }
            }
            else
            if (y1 == y2)
            {
                if (encrypt)
                {
                    newY1++;
                    newY2++;
                }
                else
                {
                    newY1--;
                    newY2--;
                }
                if (newY1 == keyMatrix.GetLength(1))
                {
                    newY1 = 0;
                    newY2 = 0;
                }
                if (newY1 == -1)
                {
                    newY1 = keyMatrix.GetLength(1) - 1;
                    newY2 = keyMatrix.GetLength(1) - 1;
                }
            }
            else
            {
                newX1 = x2;
                newX2 = x1;
            }

            return (keyMatrix[newX1, newY1], keyMatrix[newX2, newY2]);
        }
        bool CharInKeyExist(char c, char[,] keyMatrix)
        {
            int size = keyMatrix.GetLength(0);
            for (int x = 0; x < size; x++)
                for (int y = 0; y < size; y++)
                    if (keyMatrix[x, y] == c)
                        return true;
            return false;
        }
        (int, int) PositionCharInKey(char c, char[,] keyMatrix)
        {
            int size = keyMatrix.GetLength(0);
            for (int x = 0; x < size; x++)
                for (int y = 0; y < size; y++)
                    if (keyMatrix[x, y] == c)
                        return (x, y);
            return (-1, -1);
        }
        /// <summary>
        /// создать ключ автоматически
        /// </summary>
        /// <returns>ключ-матрица</returns>
        public char[,] AutoGenerateKey()
        {
            int size = 0;
            while ((size + 1) * (size + 1) <= alphabet.Length)
                size++;
            char[,] keyMatrix = new char[size, size];

            List<char> tempAlphabet = new List<char>();
            tempAlphabet.AddRange(alphabet);

            for (int x = 0; x < size; x++)
                for (int y = 0; y < size; y++)
                {
                    Random r = new Random();
                    int idChar = r.Next(0, tempAlphabet.Count);
                    keyMatrix[x, y] = tempAlphabet[idChar];
                    tempAlphabet.RemoveAt(idChar);
                }
            return keyMatrix;
        }
    }
    /// <summary>
    /// Шифр Цезаря
    /// </summary>
    class CaesarsCipher/*Шифр Цезаря*/ : Ciphers
    {
        public CaesarsCipher() : base()
        {
        }
        public CaesarsCipher(TypeAlphabet type) : base(type)
        {
        }
        public CaesarsCipher(string alphabet) : base(alphabet)
        {
        }
        /// <summary>
        /// шифрование текста
        /// </summary>
        /// <param name="text">текст</param>
        /// <param name="key">ключ</param>
        /// <returns>зашифрованый текст</returns>
        public string Encrypt(string text, int key)//шифрование текста
        => CodeEncode(text, key);
        /// <summary>
        /// дешифрование текста
        /// </summary>
        /// <param name="text">текст</param>
        /// <param name="key">ключ</param>
        /// <returns>расшифрованый текст</returns>
        public string Decrypt(string text, int key)//дешифрование текста
        => CodeEncode(text, -key);
        private string CodeEncode(string text, int key)
        {
            if (key > alphabet.Length)
                throw new Exception("ключ должен быть не больше размера алфавита");
            string newText = "";
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                int index = alphabet.IndexOf(c);
                if (index < 0)
                {
                    //если символ не найден, то добавляем его в неизменном виде
                    newText += c.ToString();
                }
                else
                {
                    int codeIndex = (alphabet.Length + index + key) % alphabet.Length;
                    newText += alphabet[codeIndex];
                }
            }

            return newText;
        }
    }
    /// <summary>
    /// Шифр Виженера(модифицированый шифр Цезаря)
    /// </summary>
    class VigenerCipher/*Шифр Виженера(модифицированый шифр Цезаря)*/ : Ciphers
    {
        public VigenerCipher() : base()
        {
        }
        public VigenerCipher(TypeAlphabet type) : base(type)
        {
        }
        public VigenerCipher(string alphabet) : base(alphabet)
        {
        }
        /// <summary>
        /// шифрование текста
        /// </summary>
        /// <param name="text">текст</param>
        /// <param name="key">ключ</param>
        /// <returns>зашифрованый текст</returns>
        public string Encrypt(string text, int key)//шифрование текста
        => CodeEncode(text, key, true);
        /// <summary>
        /// дешифрование текста
        /// </summary>
        /// <param name="text">текст</param>
        /// <param name="key">ключ</param>
        /// <returns>расшифрованый текст</returns>
        public string Decrypt(string text, int key)//дешифрование текста
        => CodeEncode(text, key, false);
        private string CodeEncode(string text, int key, bool encrypt)
        {
            string strKey = key.ToString();
            string newText = "";
            int indexPositionKey = 0;
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                int indexCharInAlphabet = alphabet.IndexOf(c);
                if (indexCharInAlphabet < 0)
                {
                    //если символ не найден, то добавляем его в неизменном виде
                    newText += c.ToString();
                }
                else
                {
                    int codeIndex;
                    if (encrypt)
                        codeIndex = (alphabet.Length + indexCharInAlphabet + GetNumber(strKey[indexPositionKey])) % alphabet.Length;
                    else
                        codeIndex = (alphabet.Length + indexCharInAlphabet - GetNumber(strKey[indexPositionKey])) % alphabet.Length;

                    newText += alphabet[codeIndex];
                }
                indexPositionKey++;
                if (indexPositionKey >= strKey.Length)
                    indexPositionKey = 0;
            }
            return newText;
        }
        int GetNumber(char c)
        {
            switch (c)
            {
                case '0': return 0;
                case '1': return 1;
                case '2': return 2;
                case '3': return 3;
                case '4': return 4;
                case '5': return 5;
                case '6': return 6;
                case '7': return 7;
                case '8': return 8;
                case '9': return 9;
                default: return -1;
            }
        }
    }
    /// <summary>
    /// Шифр Виженера модифицированый
    /// </summary>
    class VigenerCipherModification/*Шифр Виженера модифицированый*/ : Ciphers
    {
        public VigenerCipherModification() : base()
        {
        }
        public VigenerCipherModification(TypeAlphabet type) : base(type)
        {
        }
        public VigenerCipherModification(string alphabet) : base(alphabet)
        {
        }
        /// <summary>
        /// шифрование текста
        /// </summary>
        /// <param name="text">текст</param>
        /// <param name="key">ключ</param>
        /// <returns>зашифрованый текст</returns>
        public string Encrypt(string text, int[] key)//шифрование текста
        => CodeEncode(text, key, true);
        /// <summary>
        /// дешифрование текста
        /// </summary>
        /// <param name="text">текст</param>
        /// <param name="key">ключ-список</param>
        /// <returns>расшифрованый текст</returns>
        public string Decrypt(string text, int[] key)//дешифрование текста
        => CodeEncode(text, key, false);
        private string CodeEncode(string text, int[] keys, bool encrypt)
        {
            for (int i = 0; i < keys.Length; i++)
                if (keys[i] > alphabet.Length)
                    throw new Exception("ключ под номером " + i + " должен быть не больше размера алфавита");

            string newText = "";
            int indexPositionKey = 0;
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                int indexCharInAlphabet = alphabet.IndexOf(c);
                if (indexCharInAlphabet < 0)
                {
                    //если символ не найден, то добавляем его в неизменном виде
                    newText += c.ToString();
                }
                else
                {
                    int codeIndex;
                    if (encrypt)
                        codeIndex = (alphabet.Length + indexCharInAlphabet + keys[indexPositionKey]) % alphabet.Length;
                    else
                        codeIndex = (alphabet.Length + indexCharInAlphabet - keys[indexPositionKey]) % alphabet.Length;

                    newText += alphabet[codeIndex];
                }
                indexPositionKey++;
                if (indexPositionKey >= keys.Length)
                    indexPositionKey = 0;
            }
            return newText;
        }
        /// <summary>
        /// автоматически создаёт ключ-список
        /// </summary>
        /// <param name="size">размер ключа-списка</param>
        /// <returns>ключ-список</returns>
        public int[] AutoGenerateKey(int size)
        {
            int[] keys = new int[size];
            for (int i = 0; i < size; i++)
            {
                Random r = new Random();
                keys[i] = r.Next(0, alphabet.Length);
            }
            return keys;
        }
    }
    #endregion
    #region сортировки
    // https://kvodo.ru/sortirovka-shella.html
    /// <summary>
    /// сортировки
    /// </summary>
    abstract class Sorting<T>
    {
        /// <summary>
        /// Сортировка
        /// </summary>
        /// <param name="array">массив, который необходимо отсортировать</param>
        /// <returns>отсортированный массив</returns>
        public abstract T[] SortingMethod(T[] array);
        /// <summary>
        /// функция обмена
        /// </summary>
        /// <param name="array">массив, в котором необходимо поменять местами элементы</param>
        /// <param name="id">индекс следующего элемента, который поменяется местами с предидущим</param>
        protected void Swap(ref float[] array, int id)// функция обмена
        {
            float temp;
            temp = array[id];
            array[id] = array[id - 1];
            array[id - 1] = temp;
        }
        /// <summary>
        /// Сортировка методом вставки
        /// </summary>
        public class SortingByInsertionMethod/*Сортировка методом вставки*/ : Sorting<float>
        {
            public override float[] SortingMethod(float[] array)
            {
                int size = array.Length;
                //int CountComparison = 0/*Количество сравнений*/, CountTransfer = 0/*Количество пересылок*/;
                float temp;
                int j;
                for (int i = 1; i < size; i++) //проходит по всем элементам
                {
                    temp = array[i];
                    j = i - 1;
                    //CountComparison++;
                    while (j >= 0 && array[j] > temp)//пересылает
                    {
                        array[j + 1] = array[j];
                        array[j] = temp;
                        j--;
                        //CountTransfer++;
                    }
                }
                return array;
            }
        }
        /// <summary>
        /// Сортировка методом Шелла
        /// </summary>
        public class SortingByTheShellMethod/*Сортировка методом Шелла*/ : Sorting<float>
        {
            public override float[] SortingMethod(float[] array)
            {
                int size = array.Length;
                int h = size / 2;//длина шагов
                                 //int CountComparison = 0/*Количество сравнений*/, CountTransfer = 0/*Количество пересылок*/;
                int j;
                float temp;
                while (h > 0)
                {
                    for (int i = 0; i < size - h; i++)
                    {
                        j = i;
                        //CountComparison++;
                        while (j >= 0 && array[j] > array[j + h])
                        {
                            temp = array[j];
                            array[j] = array[j + h];
                            array[j + h] = temp;
                            //CountTransfer++;
                            j--;
                        }
                    }
                    h /= 2;
                }
                return array;
            }
        }
        /// <summary>
        /// Сортировка по подсчёту
        /// </summary>
        public class SortingByCounting/*Сортировка по подсчёту*/ : Sorting<int>
        {
            int maxNumber;//максимальное значение в массиве (если -1, то нужно найти)
            bool find;
            public SortingByCounting(int maxNumber = -1)
            {
                find = maxNumber < 0;
                this.maxNumber = maxNumber;
            }
            public override int[] SortingMethod(int[] array)
            {
                int size = array.Length;
                if (find)
                {
                    int max = array[0];
                    for (int i = 0; i < size; i++)
                        if (array[i] > max)
                            max = array[i];
                    maxNumber = max;
                }
                int[] numbers = new int[maxNumber + 1];
                for (int i = 0; i < size; i++)
                    numbers[array[i]] = numbers[array[i]] + 1;
                int j = 0;
                for (int i = 0; i < (maxNumber + 1); i++)
                    while (numbers[i] > 0)
                    {
                        array[j] = i;
                        numbers[i]--;
                        j++;
                    }
                return array;
            }
        }
        /// <summary>
        /// Сортировка по сравнению и подсчёту
        /// </summary>
        public class SortingByComparisonAndCounting/*Сортировка по сравнению и подсчёту*/ : Sorting<float>
        {
            public override float[] SortingMethod(float[] array)
            {
                int size = array.Length;
                int[] numbers = new int[size];
                for (int i = 0; i < size - 1; i++)
                    for (int j = i + 1; j < size; j++)
                    {
                        if (array[i] < array[j])
                            numbers[j]++;
                        else
                            numbers[i]++;
                    }
                float[] newArray = new float[size];
                for (int i = 0; i < size; i++)
                    newArray[numbers[i]] = array[i];
                return newArray;
            }
        }
        /// <summary>
        /// Сортировка выбором
        /// </summary>
        public class SortingByChoice/*Сортировка выбором*/ : Sorting<float>
        {
            public override float[] SortingMethod(float[] array)
            {
                int size = array.Length;
                int key;
                float temp;
                for (int i = 0; i < size - 1; i++)
                {
                    temp = array[i];
                    key = i;
                    for (int j = i + 1; j < size; j++)
                        if (array[j] < array[key]) key = j;
                    if (key != i)
                    {
                        array[i] = array[key];
                        array[key] = temp;
                    }
                }
                return array;
            }
        }
        /// <summary>
        /// Гномья сортировка
        /// </summary>
        public class DwarfSorting/*Гномья сортировка*/ : Sorting<float>
        {

            public override float[] SortingMethod(float[] array)
            {
                int i = 1;
                int size = array.Length;
                float temp;
                while (i < size)
                {
                    if (i == 0 || array[i - 1] <= array[i])
                        i++;
                    else
                    {
                        temp = array[i];
                        array[i] = array[i - 1];
                        array[--i] = temp;
                    }
                }
                return array;
            }
        }
        /// <summary>
        /// Шейкерная сортировка (перемешиванием)
        /// </summary>
        public class ShakerSorting/*Шейкерная сортировка (перемешиванием)*/ : Sorting<float>
        {
            public override float[] SortingMethod(float[] array)
            {
                int size = array.Length;
                int left = 1, right = size - 1, i;
                while (left <= right)
                {
                    for (i = right; i >= left; i--)
                        if (array[i - 1] > array[i])
                            Swap(ref array, i);
                    left++;
                    for (i = left; i <= right; i++)
                        if (array[i - 1] > array[i])
                            Swap(ref array, i);
                    right--;
                }
                return array;
            }

        }
        /// <summary>
        /// Быстрая сортировка
        /// </summary>
        public class QuickSorting/*Быстрая сортировка*/ : Sorting<float>
        {
            public override float[] SortingMethod(float[] array)
            {
                int size = array.Length;
                QuickSort(array, 0, size - 1);
                return array;
            }

            void QuickSort(float[] array, int first, int last)//рекурсивная функция сортировки
            {
                float mid, count;
                int newFirst = first, newLast = last;
                mid = array[(newFirst + newLast) / 2]; //вычисление опорного элемента
                do
                {
                    while (array[newFirst] < mid) newFirst++;
                    while (array[newLast] > mid) newLast--;
                    if (newFirst <= newLast) //перестановка элементов
                    {
                        count = array[newFirst];
                        array[newFirst] = array[newLast];
                        array[newLast] = count;
                        newFirst++;
                        newLast--;
                    }
                } while (newFirst < newLast);
                if (first < newLast)
                    QuickSort(array, first, newLast);
                if (newFirst < last)
                    QuickSort(array, newFirst, last);
            }
        }
        /// <summary>
        /// Сортировка слиянием
        /// </summary>
        public class MergeSorting/*Сортировка слиянием*/ : Sorting<float>
        {
            int size;
            public override float[] SortingMethod(float[] array)
            {
                size = array.Length;
                return MergeSort(array, 0, size - 1);
            }
            float[] MergeSort(float[] array, int first, int last)//рекурсивная процедура сортировки
            {
                if (first < last)
                {
                    array = MergeSort(array, first, (first + last) / 2); //сортировка левой части
                    array = MergeSort(array, (first + last) / 2 + 1, last); //сортировка правой части
                    array = Merge(array, first, last); //слияние двух частей
                }
                return array;
            }
            float[] Merge(float[] array, int first, int last)
            {
                int middle, start, final, j;
                float[] tempArray = new float[size];
                middle = (first + last) / 2; //вычисление среднего элемента
                start = first; //начало левой части
                final = middle + 1; //начало правой части
                for (j = first; j <= last; j++) //выполнять от начала до конца
                    if ((start <= middle) && ((final > last) || (array[start] < array[final])))
                    {
                        tempArray[j] = array[start];
                        start++;
                    }
                    else
                    {
                        tempArray[j] = array[final];
                        final++;
                    }
                //возвращение результата в список
                for (j = first; j <= last; j++)
                    array[j] = tempArray[j];
                return array;
            }
        }
        /// <summary>
        /// Сортировка пузырьком
        /// </summary>
        public class BubbleSorting/*Сортировка пузырьком*/ : Sorting<float>
        {
            public override float[] SortingMethod(float[] array)
            {
                int size = array.Length;
                for (int i = 0; i < size - 1; i++)
                    for (int j = 0; j < size - (i + 1); j++)
                        if (array[j] > array[j + 1])
                            Swap(ref array, j + 1);
                return array;
            }
        }
    }
    #endregion
    #region помощь в разработке
    /// <summary>
    /// сборник методов, упрощающих жизнь в разработке
    /// </summary> 
    public static class HelpDevelop
    {
        /// <summary>
        /// метод визуализации массива
        /// </summary> 
        /// <returns>строка визуализации</returns>
        public static string ShowArray<T>(this T[] arr)
        {
            string str = "";
            for (int i = 0; i < arr.Length; i++)
                str += arr[i] + "\n";
            return str;
        }
        /// <summary>
        /// метод визуализации 2D массива
        /// </summary>
        /// <returns>строка визуализации</returns>
        public static string ShowArray<T>(this T[,] arr)
        {
            string str = "";
            for (int x = 0; x < arr.GetLength(0); x++)
            {
                for (int y = 0; y < arr.GetLength(1); y++)
                    str += arr[x, y] + " ";
                str += "\n";
            }
            return str;
        }
        /// <summary>
        /// метод перемешивания массива
        /// </summary>
        /// <returns>перемешанный массив</returns>
        public static T[] MixingArray<T>(this T[] arr)
        {
            T temp;
            for (int id = 0; id < arr.Length; id++)
            {
                Random rand = new Random();
                int idRand = rand.Next(id, arr.Length);
                temp = arr[id];
                arr[id] = arr[idRand];
                arr[idRand] = temp;
            }
            return arr;
        }
        /// <summary>
        /// метод перемешивания 2D массива
        /// </summary>
        /// <returns>перемешанный 2D массив</returns>
        public static T[,] MixingArray<T>(this T[,] arr)
        {
            T temp;
            for (int x = 0; x < arr.GetLength(0); x++)
            {
                for (int y = 0; y < arr.GetLength(1); y++)
                {
                    Random rand = new Random();
                    int xRand = rand.Next(x, arr.GetLength(0));
                    int yRand = rand.Next(y, arr.GetLength(1));
                    temp = arr[x, y];
                    arr[x, y] = arr[xRand, yRand];
                    arr[xRand, yRand] = temp;
                }
            }
            return arr;
        }
        #region конвертации
        public static float[] ToPercent(this float[] arr)
        {
            int length = arr.Length;
            float summ = 0;
            for (int i = 0; i < length; i++)
                summ += arr[i];
            for (int i = 0; i < length; i++)
                arr[i] *= 100f / summ;
            return arr;
        }
        public static float ToFloat(this string s)
        {
            return Convert.ToSingle(s.Replace('.', ','));
        }
        public static float[] ToFloat(this string[] s)
        {
            float[] sFloat = new float[s.Length];
            for (int i = 0; i < s.Length; i++)
            {
                sFloat[i] = Convert.ToSingle(s[i].Replace('.', ','));
            }
            return sFloat;
        }
        public static float[] ToFloat(this int[] arr)
        {
            float[] arrFloat = new float[arr.Length];
            for (int i = 0; i < arr.Length; i++)
                arrFloat[i] = arr[i];
            return arrFloat;
        }
        public static int ToInt(this string s)
        {
            return Convert.ToInt32(s);
        }
        public static int[] ToInt(this string[] s)
        {
            int[] sInt = new int[s.Length];
            for (int i = 0; i < s.Length; i++)
            {
                sInt[i] = Convert.ToInt32(s[i]);
            }
            return sInt;
        }
        public static bool ToBool(this string s)
        {
            if (s == "true" || s == "1")
                return true;
            else
                return false;
        }
        public static bool[] ToArrBool(this string s)
        {
            bool[] Be = new bool[s.Length];
            for (int i = 0; i < s.Length; i++)
                Be[i] = s[i] == '1' ? true : false;
            return Be;
        }
        #endregion
    }
    #endregion
    #region Вычислительная математика
    public delegate double Function(double x);
    delegate double BigFunction(double[] arg);
    public delegate double DifferentialEquations(double x, double u);
    #region Решение алгебраических и трансцендентных уравнений(нахождение нуля функции)
    //ещё есть метод Брента, метод Риддера, метод простых итераций
    /// <summary>
    /// Решение алгебраических и трансцендентных уравнений(нахождение нуля функции)
    /// </summary>
    abstract class TheRootsOfTheEquation//Работает корректно если один корень ур. на интервале [min, max]
    {
        protected double min, max, E;
        /// <summary>
        /// конструктор решения алгебраических и трансцендентных уравнений(нахождение нуля функции)
        /// </summary>
        /// <param name="min">левая граница</param>
        /// <param name="max">правая граница</param>
        /// <param name="E">точность</param>
        public TheRootsOfTheEquation(double min, double max, double E = 10E-5)
        {
            this.E = E;
            if (min >= max)
                throw new Exception("левая граница должна быть меньше по значению, чем правая граница");
            this.min = min;
            this.max = max;
        }
        /// <summary>
        /// метод решения алгебраических и трансцендентных уравнений(нахождение нуля функции)
        /// </summary>
        /// <param name="f">уравнение</param>
        /// <returns>координата нуля функции</returns>
        public abstract double Method(Function f);
        /// <summary>
        /// метод решения алгебраических и трансцендентных уравнений(нахождение нуля функции) с подсчётом итераций
        /// </summary>
        /// <param name="f">уравнение</param>
        /// <param name="iterations">количество итераций потрченных на решение</param>
        /// <returns>координата нуля функции</returns>
        public abstract double Method(Function f, out ulong iterations);
        /// <summary>
        /// Метод Ньютона
        /// </summary>
        public class Newton /*Метод Ньютона*/: TheRootsOfTheEquation
        {
            /// <summary>
            /// конструктор решения алгебраических и трансцендентных уравнений(нахождение нуля функции) методом Ньютона
            /// </summary>
            public Newton(double min, double max, double E = 10E-5) : base(min, max, E)
            {
            }
            public override double Method(Function f)
            {
                double x = (max + min) / 2, h = f(min) * E / (f(min + E) - f(min));
                while (Math.Abs(f(x)) > E)
                {
                    x -= h;
                    h = f(x) * E / (f(x + E) - f(x));
                }
                return x;
            }
            public override double Method(Function f, out ulong iterations)
            {
                iterations = 1;
                double x = (max + min) / 2, h = f(min) * E / (f(min + E) - f(min));
                while (Math.Abs(f(x)) > E)
                {
                    x -= h;
                    h = f(x) * E / (f(x + E) - f(x));
                    iterations++;
                }
                return x;
            }
        }
        /// <summary>
        /// Метод секущих
        /// </summary>
        public class Secant /*Метод секущих*/: TheRootsOfTheEquation
        {
            /// <summary>
            /// конструктор решения алгебраических и трансцендентных уравнений(нахождение нуля функции) методом секущих
            /// </summary>
            public Secant(double min, double max, double E = 10E-5) : base(min, max, E)
            {
            }
            public override double Method(Function f)
            {
                double x1 = min, x2 = min + E, h = (x2 - x1) * f(x2) / (f(x2) - f(x1));
                while (Math.Abs(f(x2)) > E)
                {
                    x1 = x2;
                    x2 -= h;
                    h = (x2 - x1) * f(x2) / (f(x2) - f(x1));
                }
                return x2;
            }
            public override double Method(Function f, out ulong iterations)
            {
                iterations = 1;
                double x1 = min, x2 = min + E, h = (x2 - x1) * f(x2) / (f(x2) - f(x1));
                while (Math.Abs(f(x2)) > E)
                {
                    x1 = x2;
                    x2 -= h;
                    h = (x2 - x1) * f(x2) / (f(x2) - f(x1));
                    iterations++;
                }
                return x2;
            }
        }
        /// <summary>
        /// Метод Мюллера
        /// </summary>
        public class Muller /*Метод Мюллера*/: TheRootsOfTheEquation
        {
            /// <summary>
            /// конструктор решения алгебраических и трансцендентных уравнений(нахождение нуля функции) методом Мюллера
            /// </summary>
            public Muller(double min, double max, double E = 0.0001) : base(min, max, E)
            {
            }
            public override double Method(Function f)
            {
                double x, x1 = min, x2 = (max + min) / 2, x3 = max, d1, d2, h1, h2, A, B, C, D, Answer1, Answer2;
                #region формулы Мюллера
                d1 = f(x1) - f(x3);
                d2 = f(x2) - f(x3);
                h1 = x1 - x3;
                h2 = x2 - x3;
                A = f(x3);
                B = ((d2 * h1 * h1) - (d1 * h2 * h2)) / ((h1 * h2) * (h1 - h2));
                C = ((d1 * h2) - (d2 * h1)) / ((h1 * h2) * (h1 - h2));
                #endregion
                D = Math.Abs(Math.Sqrt(B * B - 4 * A * C));
                Answer1 = (-2 * A) / (B + D);
                Answer2 = (-2 * A) / (B - D);
                if (Answer1 > Answer2)
                    x = Answer1 + x3;
                else
                    x = Answer2 + x3;
                x1 = x2;
                x2 = x3;
                x3 = x;
                while (Math.Abs(f(x)) > E)
                {
                    #region формулы Мюллера
                    d1 = f(x1) - f(x3);
                    d2 = f(x2) - f(x3);
                    h1 = x1 - x3;
                    h2 = x2 - x3;
                    A = f(x3);
                    B = ((d2 * h1 * h1) - (d1 * h2 * h2)) / ((h1 * h2) * (h1 - h2));
                    C = ((d1 * h2) - (d2 * h1)) / ((h1 * h2) * (h1 - h2));
                    #endregion
                    D = Math.Abs(Math.Sqrt(B * B - 4 * A * C));
                    Answer1 = (-2 * A) / (B + D);
                    Answer2 = (-2 * A) / (B - D);
                    if (Answer1 > Answer2)
                        x = Answer1 + x3;
                    else
                        x = Answer2 + x3;
                    x1 = x2;
                    x2 = x3;
                    x3 = x;
                }
                return x;
            }
            public override double Method(Function f, out ulong iterations)
            {
                iterations = 1;
                double x, x1 = min, x2 = (max + min) / 2, x3 = max, d1, d2, h1, h2, A, B, C, D, Answer1, Answer2;
                #region формулы Мюллера
                d1 = f(x1) - f(x3);
                d2 = f(x2) - f(x3);
                h1 = x1 - x3;
                h2 = x2 - x3;
                A = f(x3);
                B = ((d2 * h1 * h1) - (d1 * h2 * h2)) / ((h1 * h2) * (h1 - h2));
                C = ((d1 * h2) - (d2 * h1)) / ((h1 * h2) * (h1 - h2));
                #endregion
                D = Math.Abs(Math.Sqrt(B * B - 4 * A * C));
                Answer1 = (-2 * A) / (B + D);
                Answer2 = (-2 * A) / (B - D);
                if (Answer1 > Answer2)
                    x = Answer1 + x3;
                else
                    x = Answer2 + x3;
                x1 = x2;
                x2 = x3;
                x3 = x;
                while (Math.Abs(f(x)) > E)
                {
                    #region формулы Мюллера
                    d1 = f(x1) - f(x3);
                    d2 = f(x2) - f(x3);
                    h1 = x1 - x3;
                    h2 = x2 - x3;
                    A = f(x3);
                    B = ((d2 * h1 * h1) - (d1 * h2 * h2)) / ((h1 * h2) * (h1 - h2));
                    C = ((d1 * h2) - (d2 * h1)) / ((h1 * h2) * (h1 - h2));
                    #endregion
                    D = Math.Abs(Math.Sqrt(B * B - 4 * A * C));
                    Answer1 = (-2 * A) / (B + D);
                    Answer2 = (-2 * A) / (B - D);
                    if (Answer1 > Answer2)
                        x = Answer1 + x3;
                    else
                        x = Answer2 + x3;
                    x1 = x2;
                    x2 = x3;
                    x3 = x;
                    iterations++;
                }
                return x;
            }
        }
    }


    #endregion
    #region Решение обыкновенных дифференциальных уравнений и систем (вычисление точек интеграла уравнений)
    //ещё есть многошаговые методы Адамса, явный и неявный метод Эйлера
    /// <summary>
    /// Решение обыкновенных дифференциальных уравнений и систем (вычисление точек интеграла уравнений)
    /// </summary>
    public abstract class Integral
    {
        protected double a, b, h;
        protected int size;
        /// <summary>
        /// конструктор решения обыкновенных дифференциальных уравнений и систем (вычисление точек интеграла уравнений)
        /// </summary>
        /// <param name="a">левая граница</param>
        /// <param name="b">правая граница</param>
        /// <param name="h">шаг</param>
        public Integral(double a, double b, double h = 0.1)
        {
            if (a >= b)
                throw new Exception("левая граница должна быть меньше по значению, чем правая граница");
            this.a = a;
            this.b = b;
            this.h = h;
            size = (int)((b - a) / h) + 1;
        }
        /// <summary>
        /// метод решения обыкновенных дифференциальных уравнений и систем (вычисление точек интеграла уравнений)
        /// </summary>
        /// <param name="f">уравнение</param>
        /// <param name="u0">начальное значение</param>
        /// <returns>матрица</returns>
        public abstract double[,] Method(DifferentialEquations f, double u0);
        /// <summary>
        /// Метод 1
        /// </summary>
        public class UnknowMethod/*Метод неизвестный*/ : Integral
        {
            /// <summary>
            /// конструктор решения обыкновенных дифференциальных уравнений и систем (вычисление точек интеграла уравнений) методом неизвестным
            /// </summary>
            public UnknowMethod(double a, double b, double h = 0.1) : base(a, b, h)
            {
            }
            public override double[,] Method(DifferentialEquations f, double u0)
            {
                double u1, k1, k2, k3, k4, t = a;
                double[,] mattrix = new double[size, 2];
                mattrix[0, 0] = u0;
                mattrix[0, 1] = t;
                int id = 1;
                do
                {

                    k1 = h * f(t, u0);
                    k2 = h * f(t + h / 2, u0 + k1 / 2);
                    k3 = h * f(t + h / 2, u0 + k2 / 2);
                    k4 = h * f(t + h, u0 + k3);
                    u1 = u0 + (k1 + 2 * k2 + 2 * k3 + k4) / 6;
                    t += h;
                    u0 = u1;

                    mattrix[id, 0] = u0;
                    mattrix[id, 1] = t;
                    id++;
                } while (t < b);
                return mattrix;
            }
        }
        /// <summary>
        /// Метод Хойна
        /// </summary>
        public class Hoina/*Метод Хойна*/ : Integral
        {
            public Hoina(double a, double b, double h = 0.1) : base(a, b, h)
            {
            }

            public override double[,] Method(DifferentialEquations f, double u0)
            {
                double u1, k1, k2, k3, t = a;
                double[,] mattrix = new double[size, 2];
                mattrix[0, 0] = u0;
                mattrix[0, 1] = t;
                int id = 1;
                do
                {

                    k1 = h * f(t, u0);
                    k2 = h * f(t + h / 3, u0 + k1 / 3);
                    k3 = h * f(t + h * 2 / 3, u0 + k2 * 2 / 3);
                    u1 = u0 + (k1 / 4 + k2 * 0 + k3 * 3 / 4);
                    t += h;
                    u0 = u1;

                    mattrix[id, 0] = u0;
                    mattrix[id, 1] = t;
                    id++;
                } while (t < b);
                return mattrix;
            }
        }
        /// <summary>
        /// Метод Рунге — Кутты
        /// </summary>
        public class RungeKutty/*Метод Рунге — Кутты*/ : Integral
        {
            /// <summary>
            /// конструктор решения обыкновенных дифференциальных уравнений и систем (вычисление точек интеграла уравнений) методом Рунге — Кутты
            /// </summary>
            public RungeKutty(double a, double b, double h = 0.1) : base(a, b, h)
            {
            }
            public override double[,] Method(DifferentialEquations f, double u0)
            {
                double u1, k1, k2, k3, t = a;
                double[,] mattrix = new double[size, 2];
                mattrix[0, 0] = u0;
                mattrix[0, 1] = t;
                int id = 1;
                do
                {

                    k1 = h * f(t, u0);
                    k2 = h * f(t + h / 2, u0 + k1 / 2);
                    k3 = h * f(t + h, u0 + k2);
                    u1 = u0 + (k1 + 4 * k2 + k3) / 6;
                    t += h;
                    u0 = u1;

                    mattrix[id, 0] = u0;
                    mattrix[id, 1] = t;
                    id++;
                } while (t < b);
                return mattrix;
            }
        }
    }
    #endregion
    #region Решение систем нелинейных уравнений
    //ещё метод Бройдена
    /// <summary>
    /// Решение систем нелинейных уравнений
    /// </summary>
    abstract class NonlinearSystemsOfEquations
    {
        protected BigFunction[]? functions;
        protected double h, E;
        /// <summary>
        ///Конструктор решения систем нелинейных уравнений
        /// </summary>
        /// <param name="h"></param>
        /// <param name="E"></param>
        public NonlinearSystemsOfEquations(double h = 0.85, double E = 10E-10)
        {
            this.h = h;
            this.E = E;
        }
        protected double Determinant(BigFunction func, double[] arg, int indexArg)//производна¤
        {
            double h = 0.00001;
            double[] NewArg = new double[arg.Length];
            for (int i = 0; i < arg.Length; i++)
                NewArg[i] = arg[i];
            NewArg[indexArg] += h;
            return (func(arg) - func(NewArg)) / h;
        }
        protected void InverseM(ref double[,] Arr)//обратна¤ матрица
        {
            int dx, dy;
            double MainOpr = OprM(Arr);
            double[,] NewArr = new double[Arr.GetLength(0), Arr.GetLength(1)];
            double[,] TempArr = new double[Arr.GetLength(0) - 1, Arr.GetLength(1) - 1];

            for (int ySkip = 0; ySkip < Arr.GetLength(0); ySkip++)
                for (int xSkip = 0; xSkip < Arr.GetLength(0); xSkip++)
                {
                    dx = 0;
                    dy = 0;
                    for (int y = 0; y < Arr.GetLength(0); y++)
                        for (int x = 0; x < Arr.GetLength(0); x++)
                        {
                            if (y != ySkip && x != xSkip)
                            {
                                TempArr[dx, dy] = Arr[x, y];
                                dx++;
                                if (dx == (Arr.GetLength(0) - 1))
                                {
                                    dx = 0;
                                    dy++;
                                }
                            }
                        }

                    NewArr[xSkip, ySkip] = AlgebraicAddition(xSkip, ySkip, TempArr) / MainOpr;
                }
            for (int y = 0; y < Arr.GetLength(0); y++)
                for (int x = 0; x < Arr.GetLength(0); x++)
                    Arr[x, y] = NewArr[x, y];
            TransposeM(ref Arr);
        }
        private double AlgebraicAddition(double xSkip, double ySkip, double[,] Arr)//алгебраические дополнение
        {
            return Math.Pow(-1, xSkip + ySkip) * OprM(Arr);
        }
        protected double OprM(double[,] a)//определитель матрицы
        {
            double rez = 0;
            int size = a.GetLength(0);
            if (size > 2)
            {
                double[,] b = new double[size - 1, size - 1];
                int bx = 0, by = 0;
                for (int i = 0; i < size; i++)
                {
                    bx = 0;
                    for (int x = 0; x < size; x++)
                        if (x != i)
                        {
                            by = 0;
                            for (int y = 1; y < size; y++)
                            {
                                b[bx, by] = a[x, y];
                                by++;
                            }
                            bx++;
                        }
                    rez += Math.Pow(-1, i) * a[i, 0] * OprM(b);
                }
            }
            else
            {
                if (size > 1)
                    rez = a[0, 0] * a[1, 1] - a[1, 0] * a[0, 1];
                else
                    rez = a[0, 0];
            }
            return rez;
        }
        protected void MultiplicationM(ref double[] rezArr, double[,] H0, double[] F)//перемножение матриц
        {
            for (int y = 0; y < F.Length; y++)
                for (int x = 0; x < F.Length; x++)
                    rezArr[y] = 0;
            for (int y = 0; y < F.Length; y++)
                for (int x = 0; x < F.Length; x++)
                    rezArr[y] += h * H0[x, y] * F[x];
        }
        protected void TransposeM(ref double[,] Arr)//транспонирование матрицы
        {
            double[,] TempArr = new double[Arr.GetLength(0), Arr.GetLength(0)];
            for (int y = 0; y < Arr.GetLength(0); y++)
                for (int x = 0; x < Arr.GetLength(0); x++)
                    TempArr[x, y] = Arr[x, y];
            for (int y = 0; y < Arr.GetLength(0); y++)
                for (int x = 0; x < Arr.GetLength(0); x++)
                    Arr[x, y] = TempArr[y, x];
        }
        protected void CreateH0(ref double[,] H0, double[] arg)//матрица якоби
        {
            for (int y = 0; y < functions.Length; y++)
                for (int x = 0; x < arg.Length; x++)
                    H0[x, y] = -Determinant(functions[y], arg, x);


        }
        /// <summary>
        /// метод решения систем нелинейных уравнений
        /// </summary>
        /// <param name="functions">уравнения</param>
        /// <param name="arguments">начальное значение неизвестных</param>
        /// <returns>конечное значение неизвестных</returns>
        public abstract double[] Method(BigFunction[] functions, double[]? arguments = null);
        public class MethodNewthon : NonlinearSystemsOfEquations
        {
            /// <summary>
            /// Конструктор решения систем нелинейных уравнений методом Ньютона
            /// </summary>
            /// <param name="h"></param>
            /// <param name="E"></param>
            public MethodNewthon(double h = 0.85, double E = 0.000000001) : base(h, E)
            {
            }
            /// <summary>
            /// метод Ньютона решения систем нелинейных уравнений
            /// </summary>
            /// <param name="functions">уравнения</param>
            /// <param name="arguments">начальное значение неизвестных</param>
            /// <returns>конечное значение неизвестных</returns>
            public override double[] Method(BigFunction[] functions, double[]? arguments = null)
            {
                this.functions = new BigFunction[functions.Length];
                for (int i = 0; i < functions.Length; i++)
                    this.functions[i] = functions[i];
                if (arguments == null)
                {
                    arguments = new double[functions.Length];//аргументы функций 
                    for (int i = 0; i < functions.Length; i++)
                        arguments[i] = 0;//начальное положение
                }
                if (arguments.Length != functions.Length)
                    throw new ArgumentException("количество неизвестных должно быть равно количеству уравнений в системе");
                double[,] J = new double[functions.Length, functions.Length];//матрица якоби
                double[] Y = new double[functions.Length];//массив функций
                double[] D = new double[functions.Length];
                double qMax;

                do
                {
                    CreateH0(ref J, arguments);
                    InverseM(ref J);

                    for (int i = 0; i < arguments.Length; i++)
                        Y[i] = functions[i](arguments);

                    MultiplicationM(ref D, J, Y);
                    for (int i = 0; i < arguments.Length; i++)
                        arguments[i] -= D[i];

                    #region нахождение максимального значени¤ массива функций 
                    qMax = 0;
                    for (int i = 0; i < functions.Length; i++)
                        if (qMax < Math.Abs(functions[i](arguments)))
                            qMax = Math.Abs(functions[i](arguments));
                    #endregion
                } while (qMax > E);

                return arguments;
            }
        }
    }
    #endregion
    #region Численное дифференцирование(производная(нахождение нуля в функции))
    /// <summary>
    /// Численное дифференцирование(производная(нахождение нуля в функции))
    /// </summary>
    public abstract class Derivatives//производная
    {
        protected double h, E;
        /// <summary>
        /// Конструктор нахождения производной
        /// </summary>
        /// <param name="h">шаг</param>
        /// <param name="E">точность</param>
        public Derivatives(double h = 10E-7, double E = 10E-5)
        {
            this.h = h;
            this.E = E;
        }
        /// <summary>
        /// Метод нахождения производной
        /// </summary>
        /// <param name="function">функция</param>
        /// <param name="x">координата x</param>
        /// <returns>значение производной</returns>
        public abstract double Method(Function function, double x);
        /// <summary>
        /// Метод нахождения производной
        /// </summary>
        /// <param name="function">функция</param>
        /// <param name="a">левая граница</param>
        /// <param name="b">правая граница</param>
        /// <param name="x">координата x при нуле функции</param>
        /// <returns>значение производной (нуль в функции)</returns>
        public abstract double Method(Function function, double a, double b, out double x);
        /// <summary>
        /// Численное дифференцирование(производная(нахождение нуля в функции)) методом центральных разностей
        /// </summary>
        public class CentralDifferences/*метод центральных разностей*/ : Derivatives
        {
            /// <summary>
            /// Конструктор нахождения производной методом центральных разностей
            /// </summary>
            /// <param name="h">шаг</param>
            /// <param name="E">точность</param>
            public CentralDifferences(double h = 10E-7, double E = 10E-5) : base(h, E)
            {
            }
            /// <summary>
            /// Нахождения производной методом центральных разностей
            /// </summary>
            /// <param name="function">функция</param>
            /// <param name="x">координата x</param>
            /// <returns>значение производной</returns>
            public override double Method(Function function, double x)
            {
                double derivative;
                derivative = (function(x) - function(x - h)) / h;
                return derivative;
            }
            /// <summary>
            /// Нахождения производной методом центральных разностей
            /// </summary>
            /// <param name="function">функция</param>
            /// <param name="a">левая граница</param>
            /// <param name="b">правая граница</param>
            /// <param name="x">координата x при нуле функции</param>
            /// <returns>значение производной (нуль в функции)</returns>
            public override double Method(Function function, double a, double b, out double x)
            {
                double x0 = a, derivative;
                do
                {
                    derivative = (function(x0) - function(x0 - h)) / h;
                    x = x0 - function(x0) / derivative;
                    if (E < Math.Abs(x - x0) || function(x) == 0)
                        x0 = x;
                } while (x0 <= b && E > Math.Abs(x - x0) && function(x) != 0);
                return derivative;
            }
        }
        /// <summary>
        /// Численное дифференцирование(производная(нахождение нуля в функции)) методом конечных разностей
        /// </summary>
        public class FiniteDifferences/*метод центральных разностей*/ : Derivatives
        {
            /// <summary>
            /// Конструктор нахождения производной методом конечных разностей
            /// </summary>
            /// <param name="h">шаг</param>
            /// <param name="E">точность</param>
            public FiniteDifferences(double h = 10E-7, double E = 10E-5) : base(h, E)
            {
            }
            /// <summary>
            /// Нахождения производной методом конечных разностей
            /// </summary>
            /// <param name="function">функция</param>
            /// <param name="x">координата x</param>
            /// <returns>значение производной</returns>
            public override double Method(Function function, double x)
            {
                double derivative;
                derivative = (function(x + h) - function(x - h)) / (2 * h);
                return derivative;
            }
            /// <summary>
            /// Метод нахождения производной методом конечных разностей
            /// </summary>
            /// <param name="function">функция</param>
            /// <param name="a">левая граница</param>
            /// <param name="b">правая граница</param>
            /// <param name="x">координата x при нуле функции</param>
            /// <returns>значение производной (нуль в функции)</returns>
            public override double Method(Function function, double a, double b, out double x)
            {
                double x0 = a, derivative;
                do
                {
                    derivative = (function(x0 + h) - function(x0 - h)) / (2 * h);
                    x = x0 - function(x0) / derivative;
                    if (E < Math.Abs(x - x0) || function(x) == 0)
                        x0 = x;
                } while (x0 <= b && E > Math.Abs(x - x0) && function(x) != 0);
                return derivative;
            }
        }
    }
    #endregion
    #endregion
}