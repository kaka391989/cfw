import React, { type PropsWithChildren } from "react";

import { useForm } from "@refinedev/antd";
import { useNavigation } from "@refinedev/core";

import { Col, DatePicker, Form, Input, Modal, Row } from "antd";

import vi from "antd/es/date-picker/locale/vi_VN";

import { useTranslation } from "react-i18next";

export const EmployeeCreatePage: React.FC<PropsWithChildren> = ({
  children,
}) => {
  const { list } = useNavigation();
  const { formProps, saveButtonProps, onFinish } = useForm({
    redirect: "list",
    dataProviderName: "odata",
  });

  const {
    i18n: { t },
  } = useTranslation();

  const buddhistLocale: typeof vi = {
    ...vi,
    lang: {
      ...vi.lang,
      fieldDateFormat: "DD/MM/YYYY",
      fieldDateTimeFormat: "DD/MM/YYYY HH:mm",
      yearFormat: "YYYY",
      cellYearFormat: "YYYY",
    },
  };

  return (
    <>
      <Modal
        open
        title={t("employees.add")}
        style={{ display: false ? "none" : "inherit" }}
        onCancel={() => {
          list("employees", "replace");
        }}
        cancelText={t("buttons.cancel")}
        okText={t("buttons.save")}
        okButtonProps={{
          ...saveButtonProps,
        }}
        width={"70%"}
      >
        <Form
          layout="vertical"
          {...formProps}
          onFinish={(values) => {
            onFinish({
              ...values,
            });
          }}
        >
          <Row gutter={16}>
            <Col span={12}>
              <Form.Item
                label={t("employees.employeeName")}
                name="employeeName"
                rules={[
                  {
                    required: true,
                  },
                ]}
              >
                <Input />
              </Form.Item>
            </Col>

            <Col span={12}>
              <Form.Item
                label={t("employees.employeeId")}
                name="employeeId"
                rules={[
                  {
                    required: true,
                  },
                ]}
              >
                <Input />
              </Form.Item>
            </Col>
          </Row>

          <Row gutter={16}>
            <Col span={12}>
              <Form.Item
                label={t("employees.email")}
                name="email"
                rules={[
                  {
                    required: true,
                  },
                ]}
              >
                <Input />
              </Form.Item>
            </Col>

            <Col span={12}>
              <Form.Item label={t("employees.joiningDate")} name="joiningDate">
                <DatePicker locale={buddhistLocale} style={{ width: "100%" }} />
              </Form.Item>
            </Col>
          </Row>

          <Row gutter={16}>
            <Col span={12}>
              <Form.Item label={t("employees.position")} name="position">
                <Input />
              </Form.Item>
            </Col>

            <Col span={12}>
              <Form.Item label={t("employees.taxId")} name="taxId">
                <Input />
              </Form.Item>
            </Col>
          </Row>

          <Row gutter={16}>
            <Col span={12}>
              <Form.Item label={t("employees.phoneNumber")} name="phoneNumber">
                <Input />
              </Form.Item>
            </Col>

            <Col span={12}>
              <Form.Item
                label={t("employees.identityNumber")}
                name="identityNumber"
              >
                <Input />
              </Form.Item>
            </Col>
          </Row>

          <Row gutter={16}>
            <Col span={12}>
              <Form.Item
                label={t("employees.directManager")}
                name="directManager"
              >
                <Input />
              </Form.Item>
            </Col>

            <Col span={12}>
              <Form.Item label={t("employees.introducer")} name="introducer">
                <Input />
              </Form.Item>
            </Col>
          </Row>
        </Form>
      </Modal>

      {children}
    </>
  );
};
