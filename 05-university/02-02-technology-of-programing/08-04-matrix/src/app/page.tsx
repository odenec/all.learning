"use client";
import { useState, useMemo } from "react";
import {
  generateMatrix,
  generateMatrixTest,
  Matrix,
  isInteger,
} from "@/lib/matrixUtils";

export default function Lab8() {
  // Состояние вкладок
  const [activeTab, setActiveTab] = useState(0);

  // Параметры матрицы
  const [N, setN] = useState(5);
  const [M, setM] = useState(5);

  // Mатрица
  const [matrix, setMatrix] = useState<Matrix>(() => generateMatrixTest(5, 5));

  // Ошибки
  const [errorBehavior, setErrorBehavior] = useState<"skip" | "fatal">("skip");
  const [fatalError, setFatalError] = useState<string | null>(null);

  // Checkbox/Radio
  const [filters, setFilters] = useState({
    integers: false,
    positive: false,
    negative: false,
    zeros: false,
    showAll: false,
  });

  // Изменение размеров
  const handleSizeChange = (newN: number, newM: number) => {
    setN(newN);
    setM(newM);
    setMatrix(generateMatrix(newN, newM));
    setFatalError(null);
  };

  // Редактирование ячейки
  const updateCell = (r: number, c: number, val: string) => {
    if (val === "") {
      const newMatrix = [...matrix];
      newMatrix[r][c] = "";
      setMatrix(newMatrix);
      return;
    }

    const regex = /^-?\d*\.?\d*$/;

    if (regex.test(val)) {
      const newMatrix = [...matrix];
      newMatrix[r][c] = val;
      setMatrix(newMatrix);
      // Сбрасываем фатальную ошибку, если пользователь исправил поле
      setFatalError(null);
    }
  };

  // Логика фильтрации
  const filteredMatrix = useMemo(() => {
    if (fatalError) return [];
    const hasActiveFilters =
      filters.integers || filters.positive || filters.negative || filters.zeros;
    return matrix.map((row) =>
      row.map((cell) => {
        const num = parseFloat(cell.toString());

        // Обработка ошибок
        if (isNaN(num)) {
          if (errorBehavior === "fatal") {
            setFatalError(
              "Критическая ошибка: в матрице некорректное значение!",
            );
            return null;
          }
          return null; // Пропускаем
        }

        if (filters.showAll || !hasActiveFilters) return num;

        // фильтрация
        let match = false;
        if (filters.integers && isInteger(num)) match = true;
        if (filters.positive && num > 0) match = true;
        if (filters.negative && num < 0) match = true;
        if (filters.zeros && num === 0) match = true;

        return match ? num : null;
      }),
    );
  }, [matrix, filters, errorBehavior, fatalError]);

  const toggleFilter = (key: keyof typeof filters) => {
    if (key === "showAll") {
      setFilters({
        integers: false,
        positive: false,
        negative: false,
        zeros: false,
        showAll: true,
      });
      setFatalError(null);
    } else {
      setFilters((prev) => ({ ...prev, [key]: !prev[key], showAll: false }));
    }
  };

  return (
    <main className="container">
      <h2>Лабораторная работа №8</h2>

      {/* Вкладки */}
      <div className="tab-header">
        <button
          className={`tab-btn ${activeTab === 0 ? "active" : ""}`}
          onClick={() => setActiveTab(0)}
        >
          Настройки
        </button>
        <button
          className={`tab-btn ${activeTab === 1 ? "active" : ""}`}
          onClick={() => setActiveTab(1)}
        >
          Матрица
        </button>
      </div>

      {activeTab === 0 && (
        <div className="controls-grid">
          <div className="filter-group">
            <label>Размер N (строк): {N}</label>
            <input
              type="range"
              min="1"
              max="20"
              value={N}
              onChange={(e) => handleSizeChange(+e.target.value, M)}
            />
            <br />
            <label>Размер M (столбцов): {M}</label>
            <input
              type="range"
              min="1"
              max="20"
              value={M}
              onChange={(e) => handleSizeChange(N, +e.target.value)}
            />
          </div>

          <div className="filter-group">
            <strong>Действие при ошибке:</strong>
            <br />
            <label>
              <input
                type="radio"
                checked={errorBehavior === "skip"}
                onChange={() => setErrorBehavior("skip")}
              />{" "}
              Продолжить
            </label>
            <br />
            <label>
              <input
                type="radio"
                checked={errorBehavior === "fatal"}
                onChange={() => setErrorBehavior("fatal")}
              />{" "}
              Прервать (Fatal)
            </label>
          </div>
        </div>
      )}

      {activeTab === 1 && (
        <>
          <div className="filter-group" style={{ marginBottom: "15px" }}>
            <strong>Фильтрация:</strong>
            <label>
              <input
                type="checkbox"
                checked={filters.integers}
                onChange={() => toggleFilter("integers")}
              />{" "}
              Целые
            </label>
            <label>
              <input
                type="checkbox"
                checked={filters.positive}
                onChange={() => toggleFilter("positive")}
              />{" "}
              Положительные
            </label>
            <label>
              <input
                type="checkbox"
                checked={filters.negative}
                onChange={() => toggleFilter("negative")}
              />{" "}
              Отрицательные
            </label>
            <label>
              <input
                type="checkbox"
                checked={filters.zeros}
                onChange={() => toggleFilter("zeros")}
              />{" "}
              Нули
            </label>
            <label>
              <input
                type="radio"
                checked={filters.showAll}
                onChange={() => toggleFilter("showAll")}
              />{" "}
              Показать все
            </label>
          </div>

          {fatalError ? (
            <div className="error-msg">{fatalError}</div>
          ) : (
            <div className="matrix-container">
              <table>
                <tbody>
                  {matrix.map((row, r) => (
                    <tr key={r}>
                      {row.map((cell, c) => {
                        const filteredVal = filteredMatrix[r]?.[c];
                        return (
                          <td
                            key={c}
                            style={{
                              backgroundColor: (() => {
                                if (filteredVal === null) return "#f9f9f9";

                                const hasActiveFilters =
                                  filters.integers ||
                                  filters.positive ||
                                  filters.negative ||
                                  filters.zeros;

                                if (filters.showAll) return "#bbdefb";

                                if (hasActiveFilters) return "#bbdefb";

                                return "transparent";
                              })(),
                            }}
                          >
                            <input
                              type="text"
                              value={cell}
                              onChange={(e) => updateCell(r, c, e.target.value)}
                            />
                          </td>
                        );
                      })}
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </>
      )}
    </main>
  );
}
