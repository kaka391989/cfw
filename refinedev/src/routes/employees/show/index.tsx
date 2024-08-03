import React, { useState } from "react";

import { useDelete, useNavigation, useShow, useUpdate } from "@refinedev/core";
import type { GetFields } from "@refinedev/nestjs-query";

import {
  CloseOutlined,
  DeleteOutlined,
  EditOutlined,
  GlobalOutlined,
  IdcardOutlined,
  MailOutlined,
  PhoneOutlined,
  ShopOutlined,
} from "@ant-design/icons";
import {
  Button,
  Card,
  Drawer,
  Form,
  Input,
  Popconfirm,
  Select,
  Space,
  Spin,
  Typography,
} from "antd";
import dayjs from "dayjs";

import {
  CustomAvatar,
  SelectOptionWithAvatar,
  SingleElementForm,
  Text,
  TextIcon,
} from "@/components";

import styles from "./index.module.css";
import { IEmployee } from "../list";
import { useTranslation } from "react-i18next";

export const EmployeeShowPage: React.FC = () => {
  const [activeForm, setActiveForm] = useState<
    "email" | "companyId" | "jobTitle" | "phone" | "timezone"
  >();
  const { list } = useNavigation();
  const { mutate } = useUpdate<IEmployee>();
  const { mutate: deleteMutation } = useDelete<IEmployee>();
  const { queryResult } = useShow<IEmployee>({
    dataProviderName: "odata",
  });

  const { t } = useTranslation();

  const closeModal = () => {
    setActiveForm(undefined);

    list("employees");
  };

  const { data, isLoading, isError } = queryResult;

  if (isError) {
    closeModal();
    return null;
  }

  if (isLoading) {
    return (
      <Drawer
        open
        width={756}
        bodyStyle={{
          background: "#f5f5f5",
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
        }}
      >
        <Spin />
      </Drawer>
    );
  }

  const {
    id,
    employeeId,
    employeeName,
    joiningDate,
    position,
    taxId,
    phoneNumber,
    identityNumber,
    email,
    directManager,
    introducer,
  } = data?.data ?? {};

  return (
    <Drawer
      open
      onClose={() => closeModal()}
      width={756}
      bodyStyle={{ background: "#f5f5f5", padding: 0 }}
      headerStyle={{ display: "none" }}
    >
      <div className={styles.header}>
        <Button
          type="text"
          // @ts-expect-error Ant Design Icon's v5.0.1 has an issue with @types/react@^18.2.66
          icon={<CloseOutlined />}
          onClick={() => closeModal()}
        />
      </div>
      <div className={styles.container}>
        <div className={styles.name}>
          <Typography.Title
            level={3}
            style={{ padding: 0, margin: 0, width: "100%" }}
            className={styles.title}
            editable={{
              onChange(value) {
                mutate({
                  resource: "employees",
                  dataProviderName: "odata",
                  id,
                  values: {
                    employeeName: value,
                  },
                  successNotification: false,
                });
              },
              triggerType: ["text", "icon"],
              // @ts-expect-error Ant Design Icon's v5.0.1 has an issue with @types/react@^18.2.66
              icon: <EditOutlined className={styles.titleEditIcon} />,
            }}
          >
            {employeeName}
          </Typography.Title>
        </div>

        <div className={styles.form}>
          <SingleElementForm
            // @ts-expect-error Ant Design Icon's v5.0.1 has an issue with @types/react@^18.2.66
            icon={<MailOutlined className="tertiary" />}
            state={
              activeForm && activeForm === "email"
                ? "form"
                : email
                ? "view"
                : "empty"
            }
            itemProps={{
              name: "email",
              label: t("employees.email"),
            }}
            view={<Text>{email}</Text>}
            onClick={() => setActiveForm("email")}
            onUpdate={() => setActiveForm(undefined)}
            onCancel={() => setActiveForm(undefined)}
          >
            <Input defaultValue={email} />
          </SingleElementForm>
        </div>

        <div className={styles.actions}>
          <Text className="ant-text tertiary">
            {t("audit.createdAt")}: {dayjs(joiningDate).format("DD/MM/YYYY")}
          </Text>

          <Popconfirm
            title="Delete the contact"
            description="Are you sure to delete this contact?"
            onConfirm={() => {
              deleteMutation(
                {
                  id,
                  dataProviderName: "odata",
                  resource: "employees",
                },
                {
                  onSuccess: () => closeModal(),
                }
              );
            }}
            okText={t("buttons.ok")}
            cancelText="No"
          >
            {/* @ts-expect-error Ant Design Icon's v5.0.1 has an issue with @types/react@^18.2.66 */}
            <Button type="link" danger icon={<DeleteOutlined />}>
              {t("buttons.delete") +
                " " +
                t("employees.name").toLocaleLowerCase()}
            </Button>
          </Popconfirm>
        </div>
      </div>
    </Drawer>
  );
};
