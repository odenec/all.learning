"use client";

import React, { useState, useEffect } from "react";
import UserForm from "@/components/UserForm";
import UserList from "@/components/UserList";
import UserDetails from "@/components/UserDetails";
import { User } from "@/lib/models/User";
import { UserDatabase } from "@/lib/services/UserDatabase";
import {
  Save,
  FolderOpen,
  Database,
  AlertCircle,
  CheckCircle,
} from "lucide-react";

const db = new UserDatabase();

export default function HomePage() {
  const [users, setUsers] = useState<User[]>([]);
  const [selectedUser, setSelectedUser] = useState<User | null>(null);
  const [message, setMessage] = useState<{
    text: string;
    type: "success" | "error";
  } | null>(null);
  const [showLoadOptions, setShowLoadOptions] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [isInitialLoad, setIsInitialLoad] = useState(true);

  const refreshList = () => setUsers(db.getAll());

  const showMessage = (text: string, type: "success" | "error") => {
    setMessage({ text, type });
    setTimeout(() => setMessage(null), 4000);
  };

  useEffect(() => {
    const loadInitialData = async () => {
      setIsInitialLoad(true);
      try {
        const response = await fetch("/api/database");
        const result = await response.json();

        if (result.success && result.data.length > 0) {
          const loadedUsers = result.data.map((obj: any) =>
            User.fromPlainObject(obj),
          );
          db.replaceAll(loadedUsers);
          refreshList();
          showMessage(
            `📂 Загружено ${loadedUsers.length} записей из файла`,
            "success",
          );
        } else {
          console.log("Файл пуст или не найден, начинаем с пустой базы");
        }
      } catch (error) {
        console.error("Ошибка начальной загрузки:", error);
      } finally {
        setIsInitialLoad(false);
      }
    };

    loadInitialData();
  }, []);

  const handleAdd = (user: User): boolean => {
    const result = db.add(user);
    if (result.success) {
      refreshList();
      showMessage("✅ Запись успешно добавлена", "success");
      return true;
    } else {
      showMessage(`❌ ${result.error || "Ошибка добавления"}`, "error");
      return false;
    }
  };

  const handleSave = async () => {
    setIsLoading(true);
    try {
      const response = await fetch("/api/database", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          users: db.getAll().map((u) => u.toPlainObject()),
        }),
      });

      const result = await response.json();

      if (result.success) {
        showMessage(`💾 ${result.message}`, "success");
      } else {
        showMessage(`❌ ${result.error}`, "error");
      }
    } catch (error) {
      showMessage("❌ Ошибка сохранения файла", "error");
    } finally {
      setIsLoading(false);
    }
  };

  const handleLoad = async (mode: "replace" | "merge") => {
    setIsLoading(true);
    setShowLoadOptions(false);

    try {
      const response = await fetch("/api/database");
      const result = await response.json();

      if (!result.success) {
        showMessage(`❌ ${result.error}`, "error");
        return;
      }

      const loadedUsers = result.data.map((obj: any) =>
        User.fromPlainObject(obj),
      );

      if (loadedUsers.length === 0) {
        showMessage("⚠️ Файл пуст или не найден", "error");
        return;
      }

      if (mode === "replace") {
        db.replaceAll(loadedUsers);
        setSelectedUser(null);
        showMessage(
          `📂 Загружено ${loadedUsers.length} записей (замена)`,
          "success",
        );
      } else {
        const beforeCount = db.count;
        db.merge(loadedUsers);
        const added = db.count - beforeCount;
        showMessage(`📂 Добавлено ${added} новых записей`, "success");
      }

      refreshList();
    } catch (error) {
      showMessage("❌ Ошибка загрузки файла", "error");
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="min-h-screen py-8 px-4">
      <div className="container mx-auto max-w-6xl">
        {/* Заголовок */}
        <header className="mb-8 text-center">
          <div className="inline-block mb-3">
            <div className="flex items-center gap-2 px-4 py-2 bg-white/60 backdrop-blur-sm rounded-full border border-indigo-100 shadow-sm">
              <Database className="w-4 h-4 text-indigo-500" />
              <span className="text-sm font-medium text-indigo-600">
                Вариант 7
              </span>
            </div>
          </div>
          <h1 className="title-gradient text-4xl md:text-5xl font-bold mb-3">
            Управление записями
          </h1>
          <p className="text-gray-500 max-w-2xl mx-auto">
            Добавляйте, сохраняйте и загружайте данные пользователей с
            фотографиями
          </p>
        </header>

        {/* Индикатор начальной загрузки */}
        {isInitialLoad && (
          <div className="mb-6 p-4 rounded-xl backdrop-blur-sm flex items-center justify-center gap-3 bg-indigo-50/90 text-indigo-800 border border-indigo-200">
            <div className="w-5 h-5 border-2 border-indigo-500 border-t-transparent rounded-full animate-spin" />
            <span className="font-medium">Загрузка данных из файла...</span>
          </div>
        )}

        {/* Индикатор загрузки */}
        {isLoading && !isInitialLoad && (
          <div className="fixed top-4 right-4 z-50">
            <div className="bg-white/90 backdrop-blur-sm rounded-full px-4 py-2 shadow-lg border border-indigo-100">
              <div className="flex items-center gap-2">
                <div className="w-4 h-4 border-2 border-indigo-500 border-t-transparent rounded-full animate-spin" />
                <span className="text-sm text-gray-600">Загрузка...</span>
              </div>
            </div>
          </div>
        )}

        {/* Сообщения */}
        {message && (
          <div
            className={`mb-6 p-4 rounded-xl backdrop-blur-sm flex items-center gap-3 ${
              message.type === "success"
                ? "bg-emerald-50/90 text-emerald-800 border border-emerald-200"
                : "bg-rose-50/90 text-rose-800 border border-rose-200"
            }`}
          >
            {message.type === "success" ? (
              <CheckCircle className="w-5 h-5 text-emerald-500" />
            ) : (
              <AlertCircle className="w-5 h-5 text-rose-500" />
            )}
            <span className="font-medium">{message.text}</span>
          </div>
        )}

        {/* Форма */}
        <UserForm onAdd={handleAdd} />

        {/* Основной контент */}
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
          <div>
            <UserList
              users={users}
              onSelect={setSelectedUser}
              selectedUserId={selectedUser?.id}
            />

            {/* Кнопки управления файлами */}
            <div className="card mt-6">
              <div className="flex items-center gap-3 mb-4">
                <div className="p-2 bg-gradient-to-r from-emerald-500 to-teal-500 rounded-xl">
                  <FolderOpen className="w-5 h-5 text-white" />
                </div>
                <h3 className="text-lg font-bold text-gray-800">
                  Управление файлом
                </h3>
              </div>

              <div className="flex flex-wrap gap-3">
                <button
                  onClick={handleSave}
                  disabled={isLoading}
                  className="btn-primary flex items-center gap-2 bg-gradient-to-r from-emerald-600 to-teal-600 shadow-emerald-200"
                >
                  <Save className="w-4 h-4" />
                  Сохранить
                </button>

                <div className="relative">
                  <button
                    onClick={() => setShowLoadOptions(!showLoadOptions)}
                    disabled={isLoading}
                    className="btn-secondary flex items-center gap-2"
                  >
                    <FolderOpen className="w-4 h-4" />
                    Считать
                  </button>

                  {showLoadOptions && (
                    <div className="absolute top-full left-0 mt-2 bg-white rounded-xl shadow-xl border border-gray-100 overflow-hidden z-10 min-w-[200px]">
                      <button
                        onClick={() => handleLoad("replace")}
                        className="w-full px-4 py-3 text-left hover:bg-indigo-50 transition-colors flex items-center gap-2 border-b border-gray-100"
                      >
                        <span className="text-indigo-600">🔄</span>
                        Заменить текущие
                      </button>
                      <button
                        onClick={() => handleLoad("merge")}
                        className="w-full px-4 py-3 text-left hover:bg-indigo-50 transition-colors flex items-center gap-2"
                      >
                        <span className="text-indigo-600">➕</span>
                        Дополнить
                      </button>
                    </div>
                  )}
                </div>
              </div>

              <div className="mt-4 pt-4 border-t border-gray-100">
                <p className="text-sm text-gray-400 flex items-center gap-2">
                  <span>📁</span>
                  Файл:{" "}
                  <code className="bg-gray-100 px-2 py-1 rounded">
                    data/database.dat
                  </code>
                </p>
              </div>
            </div>
          </div>

          <div>
            <UserDetails user={selectedUser} />
          </div>
        </div>

        {/* Футер */}
        <footer className="mt-8 text-center">
          <div className="inline-flex items-center gap-2 px-4 py-2 bg-white/60 backdrop-blur-sm rounded-full">
            <Database className="w-4 h-4 text-indigo-400" />
            <span className="text-sm text-gray-500">
              Всего записей в базе:{" "}
              <span className="font-bold text-indigo-600">{db.count}</span>
            </span>
          </div>
        </footer>
      </div>

      {/* Клик вне меню */}
      {showLoadOptions && (
        <div
          className="fixed inset-0 z-0"
          onClick={() => setShowLoadOptions(false)}
        />
      )}
    </div>
  );
}
