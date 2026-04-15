"use client";

import React from "react";
import { User } from "@/lib/models/User";
import Image from "next/image";

interface UserDetailsProps {
  user: User | null;
}

export default function UserDetails({ user }: UserDetailsProps) {
  if (!user) {
    return null;
  }

  return (
    <div className="card mt-6">
      <h3 className="text-xl font-semibold mb-4 text-gray-800">
        Информация о пользователе
      </h3>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
        <div className="space-y-3">
          <div>
            <span className="text-sm text-gray-600">ФИО:</span>
            <p className="font-medium text-gray-800">{user.fullName}</p>
          </div>

          <div>
            <span className="text-sm text-gray-600">Город:</span>
            <p className="font-medium text-gray-800">{user.city}</p>
          </div>

          <div>
            <span className="text-sm text-gray-600">Адрес:</span>
            <p className="font-medium text-gray-800">{user.address}</p>
          </div>

          <div>
            <span className="text-sm text-gray-600">Процент:</span>
            <p className="font-medium text-gray-800">{user.percent}%</p>
          </div>
        </div>

        <div>
          <span className="text-sm text-gray-600 block mb-2">Фотография:</span>
          {user.photo ? (
            <div className="relative w-full h-48">
              <Image
                src={user.photo}
                alt={user.fullName}
                fill
                className="object-contain rounded-lg"
              />
            </div>
          ) : (
            <div className="w-full h-48 bg-gray-100 rounded-lg flex items-center justify-center">
              <span className="text-gray-400">Нет фотографии</span>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
