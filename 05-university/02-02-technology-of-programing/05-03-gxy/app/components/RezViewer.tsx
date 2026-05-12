"use client";

import { useState } from "react";
import styles from "./DataViewer.module.css";

type SelectedRow = {
  rowIndex: number;
  y: number;
  values: number[];
};

type RezData = {
  dataSetNumber: number;
  x_values: number[];
  selectedRows: SelectedRow[];
  functionExpression: string;
};

type DatasetResult = {
  dataSetNumber: number;
  data?: RezData;
  rezInfo?: { file: string; createdAt: string };
  error?: string;
};

export async function fetchRezData(
  dataSetNumbers: number[],
  rowIndices: number[] | "all",
): Promise<DatasetResult[]> {
  const payload = dataSetNumbers.map((num) => ({
    dataSetNumber: num,
    rowIndices,
  }));

  const res = await fetch("/api/read-rez", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(payload),
  });

  const json = await res.json();

  if (!json.success) {
    throw new Error(json.error || "Ошибка чтения наборов");
  }

  return json.results as DatasetResult[];
}

type RezViewerProps = {
  onClose?: () => void;
};

export default function RezViewer({ onClose }: RezViewerProps) {
  const [dataSetsInput, setDataSetsInput] = useState("1");
  const [rowsInput, setRowsInput] = useState("all");
  const [results, setResults] = useState<DatasetResult[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const loadData = async () => {
    setLoading(true);
    setError("");
    setResults([]);

    try {
      const dataSetNumbers = dataSetsInput
        .split(",")
        .map((n) => parseInt(n.trim()))
        .filter((n) => !isNaN(n) && n > 0);

      if (dataSetNumbers.length === 0) {
        setError("Введите хотя бы один номер набора (положительное число)");
        setLoading(false);
        return;
      }

      let rowIndices: number[] | "all";
      if (rowsInput.trim().toLowerCase() === "all") {
        rowIndices = "all";
      } else {
        rowIndices = rowsInput
          .split(",")
          .map((n) => parseInt(n.trim()))
          .filter((n) => !isNaN(n) && n >= 0);

        if (rowIndices.length === 0) {
          setError("Введите корректные номера строк или all");
          setLoading(false);
          return;
        }
      }

      // Получаем данные через функцию
      const fetchedResults = await fetchRezData(dataSetNumbers, rowIndices);
      setResults(fetchedResults);
    } catch (err: any) {
      console.error(err);
      setError(err.message || "Ошибка соединения");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className={styles.container}>
      <div className={styles.header}>
        <h2>📦 Чтение .rez файлов (лабa 6)</h2>

        <div className={styles.buttons}>
          <button
            onClick={loadData}
            disabled={loading}
            className={styles.button}
          >
            {loading ? "Загрузка..." : "Прочитать"}
          </button>

          {onClose && (
            <button onClick={onClose} className={styles.closeButton}>
              ✕
            </button>
          )}
        </div>
      </div>

      <div className={styles.card}>
        <p>Номера наборов (через запятую):</p>
        <input
          className={styles.button}
          value={dataSetsInput}
          onChange={(e) => setDataSetsInput(e.target.value)}
          placeholder="1,2,3"
        />

        <p>Строки (через запятую или all):</p>
        <input
          className={styles.button}
          value={rowsInput}
          onChange={(e) => setRowsInput(e.target.value)}
          placeholder="0,2,4 или all"
        />
      </div>

      {error && <p className={styles.error}>❌ {error}</p>}

      {results.map((r) => (
        <div key={r.dataSetNumber} className={styles.card}>
          <h3>📁 G{r.dataSetNumber.toString().padStart(4, "0")}.rez</h3>

          {r.error && <p className={styles.error}>❌ {r.error}</p>}

          {r.rezInfo && (
            <p>
              📅 Создан: {new Date(r.rezInfo.createdAt).toLocaleString()} |
              Файл: {r.rezInfo.file}
            </p>
          )}

          {r.data && (
            <>
              <p>🧮 Функция: {r.data.functionExpression}</p>
              <div className={styles.tableWrapper}>
                <table className={styles.table}>
                  <thead>
                    <tr>
                      <th>y\x</th>
                      {r.data.x_values.map((x, i) => (
                        <th key={i}>{x.toFixed(3)}</th>
                      ))}
                    </tr>
                  </thead>
                  <tbody>
                    {r.data.selectedRows.map((row) => (
                      <tr key={row.rowIndex}>
                        <td>{row.y.toFixed(3)}</td>
                        {row.values.map((v, i) => (
                          <td key={i}>{v.toFixed(6)}</td>
                        ))}
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </>
          )}
        </div>
      ))}
    </div>
  );
}
