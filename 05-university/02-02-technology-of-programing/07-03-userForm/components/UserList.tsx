"use client";

import React from "react";
import { User } from "@/lib/models/User";

interface UserListProps {
  users: User[];
  onSelect: (user: User) => void;
}

export default function UserList({ users, onSelect }: UserListProps) {
  return (
    <div className="card">
      <h3 className="text-xl font-semibold mb-4 text-gray-800">
        Список записей ({users.length})
      </h3>

      {users.length === 0 ? (
        <p className="text-gray-500 text-center py-4">
          Нет добавленных записей
        </p>
      ) : (
        <div className="space-y-2 max-h-96 overflow-y-auto">
          {users.map((user) => (
            <div
              key={user.id}
              onClick={() => onSelect(user)}
              className="p-3 border border-gray-200 rounded-lg cursor-pointer 
                       hover:bg-primary-50 transition duration-200"
            >
              <div className="font-medium text-gray-800">{user.fullName}</div>
              <div className="text-sm text-gray-600">{user.city}</div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
