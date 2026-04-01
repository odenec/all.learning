"use client";

import { useState } from "react";
import { GridData } from "@/lib/types";
import styles from "./DataViewer.module.css";

type DataViewerProps = {
  onClose?: () => void;
};

export default function DataViewer({ onClose }: DataViewerProps) {
  const [data, setData] = useState<GridData[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const loadData = async () => {
    setLoading(true);
    setError("");
    try {
      const res = await fetch("/api/read-data");
      const json = await res.json();
      if (json.success) {
        setData(json.data);
        console.log("Загруженные данные:", json.data); // Для отладки
      } else {
        setError(json.error || "Ошибка загрузки");
      }
    } catch (err) {
      setError("Ошибка соединения");
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className={styles.container}>
      <div className={styles.header}>
        <h2>📊 Загруженные данные из dat-файлов</h2>
        <div className={styles.buttons}>
          <button
            onClick={loadData}
            disabled={loading}
            className={styles.button}
          >
            {loading ? "Загрузка..." : "Загрузить данные"}
          </button>
          {onClose && (
            <button onClick={onClose} className={styles.closeButton}>
              ✕
            </button>
          )}
        </div>
      </div>

      {error && <p className={styles.error}>❌ {error}</p>}

      {data.length === 0 && !loading && (
        <p className={styles.empty}>Нет загруженных данных</p>
      )}

      <div className={styles.dataList}>
        {data.map((grid, idx) => (
          <div key={`grid-${idx}-${grid.dataFile}`} className={styles.card}>
            <h3>📁 {grid.dataFile}</h3>
            <p>🧮 Функция: {grid.functionExpression}</p>
            <p>📌 Вариант: {grid.variant}</p>

            {grid.x_values?.length > 0 &&
            grid.y_values?.length > 0 &&
            grid.matrix?.length > 0 ? (
              <div className={styles.tableWrapper}>
                <table className={styles.table}>
                  <thead>
                    <tr>
                      <th>y\x</th>
                      {grid.x_values.map((x, xIdx) => (
                        <th key={`x-${idx}-${xIdx}-${x}`}>{x.toFixed(3)}</th>
                      ))}
                    </tr>
                  </thead>
                  <tbody>
                    {grid.y_values.map((y, yIdx) => (
                      <tr key={`y-${idx}-${yIdx}-${y}`}>
                        <td>{y.toFixed(3)}</td>
                        {grid.matrix[yIdx]?.map((val, valIdx) => (
                          <td key={`val-${idx}-${yIdx}-${valIdx}`}>
                            {val?.toFixed(6) ?? "N/A"}
                          </td>
                        ))}
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            ) : (
              <p className={styles.warning}>⚠️ Нет данных для отображения</p>
            )}
          </div>
        ))}
      </div>
    </div>
  );
}
