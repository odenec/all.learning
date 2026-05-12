"use client";

import { useState, useEffect } from "react";
import styles from "./page.module.css";
import DataViewer from "./components/DataViewer";
import RezViewer from "./components/RezViewer";
type Result = {
  x: number;
  y: number;
  result: number;
};

type Calculation = {
  id: number;
  xCount: string;
  xValues: string[];
  yStart: string;
  yEnd: string;
  yStep: string;
  yValues: number[];
  results: Result[];
  error: string;
  xCountConfirmed: boolean;
  xGenerated: boolean;
  yGenerated: boolean;
};

type FunctionInfo = {
  expression: string;
  variant: number;
};

export default function Home() {
  const [calculationCount, setCalculationCount] = useState("");
  const [calculations, setCalculations] = useState<Calculation[]>([]);
  const [loading, setLoading] = useState(false);
  const [globalError, setGlobalError] = useState("");
  const [activeTab, setActiveTab] = useState(0);
  const [functionInfo, setFunctionInfo] = useState<FunctionInfo | null>(null);
  const [showDataViewer, setShowDataViewer] = useState(false);
  const [showRezViewer, setShowRezViewer] = useState(false);

  useEffect(() => {
    const fetchFunctionInfo = async () => {
      try {
        const res = await fetch("/api/function-info");
        const data = await res.json();
        setFunctionInfo(data);
      } catch {
        console.error("Не удалось загрузить информацию о функции");
      }
    };
    fetchFunctionInfo();
  }, []);

  const generateCalculationFields = () => {
    setGlobalError("");
    const count = parseInt(calculationCount);
    if (isNaN(count) || count <= 0) {
      setGlobalError("Введите положительное число");
      return;
    }

    const newCalculations: Calculation[] = [];
    for (let i = 0; i < count; i++) {
      newCalculations.push({
        id: i,
        xCount: "",
        xValues: [],
        yStart: "",
        yEnd: "",
        yStep: "",
        yValues: [],
        results: [],
        error: "",
        xCountConfirmed: false,
        xGenerated: false,
        yGenerated: false,
      });
    }
    setCalculations(newCalculations);
    setActiveTab(0);
  };

  const handleParamChange = (
    calcId: number,
    field: keyof Pick<Calculation, "xCount" | "yStart" | "yEnd" | "yStep">,
    value: string,
  ) => {
    setCalculations((prev) =>
      prev.map((calc) =>
        calc.id === calcId ? { ...calc, [field]: value } : calc,
      ),
    );
  };

  const confirmXCount = (calcId: number) => {
    const calc = calculations.find((c) => c.id === calcId);
    if (!calc) return;

    const count = parseInt(calc.xCount);
    if (isNaN(count) || count <= 0) {
      setCalculations((prev) =>
        prev.map((c) =>
          c.id === calcId ? { ...c, error: "Введите количество x" } : c,
        ),
      );
      return;
    }

    setCalculations((prev) =>
      prev.map((c) =>
        c.id === calcId
          ? {
              ...c,
              xValues: Array(count).fill(""),
              error: "",
              xCountConfirmed: true,
              xGenerated: false,
            }
          : c,
      ),
    );
  };

  // const validateXValues = (calc: Calculation): string | null => {
  //   for (const x of calc.xValues) {
  //     const num = parseFloat(x);
  //     if (isNaN(num)) {
  //       return "Все X должны быть числами";
  //     }
  //     if (num <= 0) {
  //       return "X должен быть > 0";
  //     }
  //     if (Math.abs(num - 1) < 1e-12) {
  //       return "X не может быть равен 1 (lg(1) = 0)";
  //     }
  //   }
  //   return null;
  // };

  const generateXFields = (calcId: number) => {
    const calc = calculations.find((c) => c.id === calcId);
    if (!calc) return;

    const validXValues = calc.xValues.filter((x) => {
      const num = parseFloat(x);
      return !isNaN(num) && num > 0 && Math.abs(num - 1) >= 1e-12;
    });

    const invalidCount = calc.xValues.length - validXValues.length;

    if (invalidCount > 0) {
      setCalculations((prev) =>
        prev.map((c) =>
          c.id === calcId
            ? {
                ...c,
                xValues: validXValues,
                error: `Пропущено ${invalidCount} невалидных X значений`,
                xGenerated: true,
              }
            : c,
        ),
      );
    } else {
      setCalculations((prev) =>
        prev.map((c) =>
          c.id === calcId ? { ...c, xGenerated: true, error: "" } : c,
        ),
      );
    }
  };
  const generateYValues = (calcId: number) => {
    const calc = calculations.find((c) => c.id === calcId);
    if (!calc) return;

    const start = parseFloat(calc.yStart);
    const end = parseFloat(calc.yEnd);
    const step = parseFloat(calc.yStep);

    if (isNaN(start) || isNaN(end) || isNaN(step)) {
      setCalculations((prev) =>
        prev.map((c) =>
          c.id === calcId ? { ...c, error: "Заполните y параметры" } : c,
        ),
      );
      return;
    }

    if (step <= 0) {
      setCalculations((prev) =>
        prev.map((c) => (c.id === calcId ? { ...c, error: "Шаг > 0" } : c)),
      );
      return;
    }

    if (start > end) {
      setCalculations((prev) =>
        prev.map((c) => (c.id === calcId ? { ...c, error: "start ≤ end" } : c)),
      );
      return;
    }

    const yValues: number[] = [];
    for (let val = start; val <= end + 1e-9; val += step) {
      yValues.push(Number(val.toFixed(10)));
    }

    setCalculations((prev) =>
      prev.map((c) =>
        c.id === calcId ? { ...c, yValues, error: "", yGenerated: true } : c,
      ),
    );
  };

  const handleXChange = (calcId: number, index: number, value: string) => {
    setCalculations((prev) =>
      prev.map((calc) =>
        calc.id === calcId
          ? {
              ...calc,
              xValues: calc.xValues.map((x, i) => (i === index ? value : x)),
              error: "",
              xGenerated: false,
            }
          : calc,
      ),
    );
  };

  const handleSubmitAll = async () => {
    setLoading(true);
    setGlobalError("");

    const calculationsData = calculations.map((calc) => ({
      x_values: calc.xValues.map((v) => parseFloat(v)),
      y_values: calc.yValues,
    }));

    try {
      const res = await fetch("/api/check-function", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ calculations: calculationsData }),
      });

      const data = await res.json();

      if (!res.ok) {
        const errorMessage = data.error || "Ошибка вычисления";
        if (data.tabIndex !== undefined) {
          setCalculations((prev) =>
            prev.map((calc, index) =>
              index === data.tabIndex ? { ...calc, error: errorMessage } : calc,
            ),
          );
        } else {
          setGlobalError(errorMessage);
        }
      } else {
        setCalculations((prev) =>
          prev.map((calc, index) => ({
            ...calc,
            results: data.data[index]?.results || [],
            error: "",
          })),
        );
      }
    } catch {
      setGlobalError("Ошибка соединения с сервером");
    }
    setLoading(false);
  };

  const isTabComplete = (calc: Calculation) => {
    return (
      calc.xGenerated &&
      calc.yGenerated &&
      calc.xValues.every((v) => v !== "") &&
      !calc.error
    );
  };

  const getTabStatus = (calc: Calculation) => {
    if (calc.error) return "error";
    if (isTabComplete(calc)) return "complete";
    if (calc.xGenerated || calc.yGenerated) return "partial";
    return "incomplete";
  };

  const getOverallStatus = () => {
    const total = calculations.length;
    const completed = calculations.filter(isTabComplete).length;
    const errors = calculations.filter((c) => c.error).length;
    const partial = calculations.filter(
      (c) => !isTabComplete(c) && (c.xGenerated || c.yGenerated) && !c.error,
    ).length;
    const incomplete = total - completed - partial - errors;

    return { total, completed, errors, partial, incomplete };
  };

  const allTabsComplete = calculations.every(isTabComplete);
  const status = getOverallStatus();

  return (
    <main className={styles.main}>
      <div className={styles.container}>
        <h1 className={styles.title}>
          🧮 G(x,y) {functionInfo && `= ${functionInfo.expression}`}
        </h1>

        <div className={styles.controlsRow}>
          <div className={styles.controlGroup}>
            <label className={styles.label}>
              Расчетов:
              <input
                type="number"
                value={calculationCount}
                onChange={(e) => setCalculationCount(e.target.value)}
                className={styles.input}
                min="1"
              />
            </label>
            <button
              onClick={generateCalculationFields}
              className={styles.button}
            >
              СОЗДАТЬ
            </button>
          </div>

          <div className={styles.controlGroup}>
            <button
              onClick={() => setShowDataViewer(!showDataViewer)}
              className={`${styles.button} ${styles.dataButton}`}
            >
              {showDataViewer ? "📂 Скрыть файлы" : "📂 Показать dat-файлы"}
            </button>
            <button
              onClick={() => setShowRezViewer(!showRezViewer)}
              className={`${styles.button} ${styles.dataButton}`}
            >
              {showRezViewer ? "📦 Скрыть rez" : "📦 Показать rez"}
            </button>
          </div>
        </div>

        {globalError && <p className={styles.error}>❌ {globalError}</p>}

        {showDataViewer && (
          <DataViewer onClose={() => setShowDataViewer(false)} />
        )}
        {showRezViewer && <RezViewer onClose={() => setShowRezViewer(false)} />}

        {calculations.length > 0 && (
          <>
            <div className={styles.statusBar}>
              <span className={styles.statusItem}>
                ✅ Готово: {status.completed}
              </span>
              <span className={styles.statusItem}>
                ⚠️ Частично: {status.partial}
              </span>
              <span className={styles.statusItem}>
                ❌ Ошибки: {status.errors}
              </span>
              <span className={styles.statusItem}>
                📝 Не заполнено: {status.incomplete}
              </span>
            </div>

            <div className={styles.tabsContainer}>
              {calculations.map((calc, index) => (
                <button
                  key={index}
                  className={`${styles.tab} ${activeTab === index ? styles.activeTab : ""} ${
                    getTabStatus(calc) === "complete" ? styles.tabComplete : ""
                  } ${getTabStatus(calc) === "error" ? styles.tabError : ""} ${
                    getTabStatus(calc) === "partial" ? styles.tabPartial : ""
                  }`}
                  onClick={() => setActiveTab(index)}
                >
                  #{index + 1}
                </button>
              ))}
            </div>

            <div className={styles.errorsContainer}>
              {calculations.map(
                (calc, idx) =>
                  activeTab === idx &&
                  calc.error && (
                    <p key={calc.id} className={styles.tabError}>
                      ❌ {calc.error}
                    </p>
                  ),
              )}
            </div>

            <div className={styles.tabContent}>
              {calculations.map((calc, idx) => (
                <div
                  key={calc.id}
                  style={{ display: activeTab === idx ? "block" : "none" }}
                >
                  <div className={styles.inputsRow}>
                    <div className={styles.inputGroup}>
                      <span className={styles.inputLabel}>X кол-во:</span>
                      <div className={styles.inputWithButton}>
                        <input
                          type="number"
                          value={calc.xCount}
                          onChange={(e) =>
                            handleParamChange(calc.id, "xCount", e.target.value)
                          }
                          className={styles.fieldInput}
                          min="1"
                        />
                        <button
                          onClick={() => confirmXCount(calc.id)}
                          className={`${styles.fieldButton} ${
                            calc.xCountConfirmed ? styles.buttonSuccess : ""
                          }`}
                        >
                          {calc.xCountConfirmed ? "✓" : "ДАЛЕЕ"}
                        </button>
                      </div>
                    </div>

                    <div className={styles.inputGroup}>
                      <span className={styles.inputLabel}>Y:</span>
                      <div className={styles.inputWithButton}>
                        <input
                          type="number"
                          value={calc.yStart}
                          onChange={(e) =>
                            handleParamChange(calc.id, "yStart", e.target.value)
                          }
                          className={styles.fieldInputSmall}
                          placeholder="start"
                        />
                        <input
                          type="number"
                          value={calc.yEnd}
                          onChange={(e) =>
                            handleParamChange(calc.id, "yEnd", e.target.value)
                          }
                          className={styles.fieldInputSmall}
                          placeholder="end"
                        />
                        <input
                          type="number"
                          value={calc.yStep}
                          onChange={(e) =>
                            handleParamChange(calc.id, "yStep", e.target.value)
                          }
                          className={styles.fieldInputSmall}
                          placeholder="step"
                        />
                        <button
                          onClick={() => generateYValues(calc.id)}
                          className={`${styles.fieldButton} ${
                            calc.yGenerated ? styles.buttonSuccess : ""
                          }`}
                        >
                          {calc.yGenerated ? "✓" : "ЗАДАТЬ"}
                        </button>
                      </div>
                    </div>
                  </div>

                  {calc.xCountConfirmed && (
                    <div className={styles.xFieldsSection}>
                      <div className={styles.xFieldsHeader}>
                        <span>X ({calc.xValues.length} шт):</span>
                        <button
                          onClick={() => generateXFields(calc.id)}
                          className={`${styles.fieldButton} ${
                            calc.xGenerated ? styles.buttonSuccess : ""
                          }`}
                        >
                          {calc.xGenerated ? "✓ ЗАДАТЬ X" : "ЗАДАТЬ X"}
                        </button>
                      </div>
                      <div className={styles.xFieldsGrid}>
                        {calc.xValues.map((x, i) => (
                          <input
                            key={i}
                            type="number"
                            value={x}
                            onChange={(e) =>
                              handleXChange(calc.id, i, e.target.value)
                            }
                            placeholder={`x${i + 1}`}
                            className={`${styles.xField} ${
                              (x === "" && calc.xGenerated) || calc.error
                                ? styles.xFieldError
                                : ""
                            }`}
                          />
                        ))}
                      </div>
                    </div>
                  )}

                  {calc.yGenerated && (
                    <p className={styles.info}>
                      ✓ Y: {calc.yValues.length} значений
                    </p>
                  )}

                  {calc.results.length > 0 && (
                    <div className={styles.resultsSection}>
                      <h4>Результаты #{idx + 1}:</h4>
                      <div className={styles.tableWrapper}>
                        <table className={styles.table}>
                          <thead>
                            <tr>
                              <th>X</th>
                              <th>Y</th>
                              <th>{functionInfo?.expression}</th>
                            </tr>
                          </thead>
                          <tbody>
                            {calc.results.map((r, i) => (
                              <tr key={i}>
                                <td>{r.x}</td>
                                <td>{r.y}</td>
                                <td>{r.result.toFixed(6)}</td>
                              </tr>
                            ))}
                          </tbody>
                        </table>
                      </div>
                    </div>
                  )}
                </div>
              ))}
            </div>

            <div className={styles.controlsRow}>
              <button
                onClick={handleSubmitAll}
                disabled={loading || !allTabsComplete}
                className={`${styles.button} ${styles.submitButton} ${
                  !allTabsComplete ? styles.buttonDisabled : ""
                }`}
              >
                {loading ? "..." : "ВЫЧИСЛИТЬ"}
              </button>
              {!allTabsComplete && (
                <p className={styles.hint}>Заполните все вкладки полностью</p>
              )}
            </div>
          </>
        )}
      </div>
    </main>
  );
}
