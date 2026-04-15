"use client";

import React, { useState } from "react";
import { User } from "@/lib/models/User";
import Image from "next/image";

interface UserFormProps {
  onAdd: (user: User) => boolean;
}

export default function UserForm({ onAdd }: UserFormProps) {
  const [formData, setFormData] = useState({
    fullName: "",
    city: "",
    address: "",
    percent: "",
    photo: null as string | null,
  });

  const [errors, setErrors] = useState<Record<string, string>>({});
  const [previewPhoto, setPreviewPhoto] = useState<string | null>(null);

  const validateForm = (): boolean => {
    const newErrors: Record<string, string> = {};

    if (!formData.fullName.trim()) {
      newErrors.fullName = "ФИО обязательно";
    } else if (formData.fullName.trim().split(/\s+/).length < 3) {
      newErrors.fullName = "Введите Имя, Фамилию и Отчество";
    }
    if (!formData.city.trim()) {
      newErrors.city = "Город обязателен";
    }
    if (!formData.address.trim()) {
      newErrors.address = "Адрес обязателен";
    }

    const percent = parseFloat(formData.percent);
    if (!formData.percent.trim()) {
      newErrors.percent = "Процент обязателен";
    } else if (isNaN(percent) || percent < 0) {
      newErrors.percent = "Процент должен быть неотрицательным числом";
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handlePhotoChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file) {
      const reader = new FileReader();
      reader.onloadend = () => {
        const base64 = reader.result as string;
        setFormData({ ...formData, photo: base64 });
        setPreviewPhoto(base64);
      };
      reader.readAsDataURL(file);
    }
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();

    if (!validateForm()) {
      return;
    }

    const user = new User(
      formData.fullName,
      formData.city,
      formData.address,
      parseFloat(formData.percent),
      formData.photo,
    );

    const success = onAdd(user);

    if (success) {
      setFormData({
        fullName: "",
        city: "",
        address: "",
        percent: "",
        photo: null,
      });
      setPreviewPhoto(null);
      setErrors({});
    }
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData({ ...formData, [name]: value });

    if (errors[name]) {
      setErrors({ ...errors, [name]: "" });
    }
  };

  const isFormValid = (): boolean => {
    return (
      formData.fullName.trim() !== "" &&
      formData.fullName.trim().split(/\s+/).length >= 3 &&
      formData.city.trim() !== "" &&
      formData.address.trim() !== "" &&
      formData.percent.trim() !== "" &&
      !isNaN(parseFloat(formData.percent)) &&
      parseFloat(formData.percent) >= 0
    );
  };

  return (
    <div className="card mb-6">
      <h2 className="text-2xl font-bold mb-6 text-gray-800">
        Добавление записи
      </h2>

      <form onSubmit={handleSubmit} className="space-y-4">
        <div>
          <label className="input-label">ФИО *</label>
          <input
            type="text"
            name="fullName"
            value={formData.fullName}
            onChange={handleChange}
            className={`input-field ${errors.fullName ? "border-red-500" : ""}`}
            placeholder="Иванов Иван Иванович"
          />
          {errors.fullName && (
            <p className="error-message">{errors.fullName}</p>
          )}
        </div>

        <div>
          <label className="input-label">Город *</label>
          <input
            type="text"
            name="city"
            value={formData.city}
            onChange={handleChange}
            className={`input-field ${errors.city ? "border-red-500" : ""}`}
            placeholder="Москва"
          />
          {errors.city && <p className="error-message">{errors.city}</p>}
        </div>

        <div>
          <label className="input-label">Адрес *</label>
          <input
            type="text"
            name="address"
            value={formData.address}
            onChange={handleChange}
            className={`input-field ${errors.address ? "border-red-500" : ""}`}
            placeholder="ул. Пушкина, д. 10"
          />
          {errors.address && <p className="error-message">{errors.address}</p>}
        </div>

        <div>
          <label className="input-label">Процент *</label>
          <input
            type="text"
            name="percent"
            value={formData.percent}
            onChange={handleChange}
            className={`input-field ${errors.percent ? "border-red-500" : ""}`}
            placeholder="0-100"
          />
          {errors.percent && <p className="error-message">{errors.percent}</p>}
        </div>

        <div>
          <label className="input-label">Фотография</label>
          <div className="flex items-center space-x-4">
            <input
              type="file"
              accept="image/*"
              onChange={handlePhotoChange}
              className="hidden"
              id="photo-upload"
            />
            <label
              htmlFor="photo-upload"
              className="btn-secondary cursor-pointer inline-block"
            >
              Обзор...
            </label>
            {previewPhoto && (
              <div className="relative w-16 h-16">
                <Image
                  src={previewPhoto}
                  alt="Preview"
                  fill
                  className="object-cover rounded-lg"
                />
              </div>
            )}
          </div>
        </div>

        <div className="pt-4">
          <button
            type="submit"
            disabled={!isFormValid()}
            className="btn-primary w-full"
          >
            Принять
          </button>
        </div>
      </form>
    </div>
  );
}
