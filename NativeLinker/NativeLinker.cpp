// NativeLinker.cpp : Defines the exported functions for the DLL application.
//

//#include "stdafx.h"

#include "messenger\callbacks.h"
#include "messenger\messenger.h"
#include "messenger\observers.h"
#include "messenger\settings.h"
#include "messenger\types.h"

using namespace messenger;



typedef void(*LoginCallback)(messenger::operation_result::Type);
class CallbackProxy : public messenger::ILoginCallback
{
public:
	LoginCallback loginCallback = NULL;
private:
	void OnOperationResult(messenger::operation_result::Type result) override {
		if (loginCallback != NULL) loginCallback(result);
		loginCallback = NULL;
	}
};

typedef void(*StatusChanged)(char* MessageId, message_status::Type);
typedef void(*MessageReceived)(char* UserId, char* Message);

/*typedef void(*StatusChanged)(const MessageId&, message_status::Type);
typedef void(*MessageReceived)(const UserId&, const Message&);*/

class MessagesObserver :public messenger::IMessagesObserver {
public:
	StatusChanged statusChanged = NULL;
	MessageReceived messageReceived = NULL;
private:
	void OnMessageStatusChanged(const MessageId& msgId, message_status::Type status)override {
		if (statusChanged != NULL) {
			statusChanged((char*)(msgId.c_str()), status);
		}
		statusChanged = NULL;
	}
	void OnMessageReceived(const UserId& senderId, const Message& msg)override {
		if (messageReceived != NULL && msg.content.type==message_content_type::Text) messageReceived((char*)(senderId.c_str()), (char*) (&msg));
		messageReceived = NULL;
	}
};

/*typedef void(*ListCallback)(int* list, int length);
typedef void(*UsersResultCallback)(messenger::operation_result::Type, const UserList&);
class CallbackRequestUser : public messenger::IRequestUsersCallback
{
public:
	UsersResultCallback callback = NULL;
private:
	void OnOperationResult(operation_result::Type result, const UserList& users) override {
		if (callback != NULL) callback(result,users);
		callback = NULL;

	}
};*/


static std::shared_ptr<messenger::IMessenger> g_messenger;
static CallbackProxy g_callbackProxy;
static MessagesObserver g_messagesObserver;
/*static CallbackRequestUser g_callbackReqUser;*/

extern "C" {
	void __declspec(dllexport) Init(char* url, unsigned short port) {
		messenger::MessengerSettings settings;
		settings.serverPort = port;
		settings.serverUrl = url;
		g_messenger = messenger::GetMessengerInstance(settings);
	}

	void __declspec(dllexport) Disconnect() {
		g_messenger->UnregisterObserver(&g_messagesObserver);
		g_messenger->Disconnect();
		g_messenger->~IMessenger();
	}

	void __declspec(dllexport) Login(char * login, char * password, LoginCallback loginCallback) {
		std::string loginStr(login);
		std::string passwordStr(password);
		g_callbackProxy.loginCallback = loginCallback;
		g_messenger->Login(loginStr, passwordStr, messenger::SecurityPolicy(), &g_callbackProxy);
	}

	/*void __declspec(dllexport) RequestActiveUsers(UsersResultCallback reqUserCallback) {
		g_callbackReqUser.callback = reqUserCallback;
		g_messenger->RequestActiveUsers(&g_callbackReqUser);
	}*/

	/*Message _declspec(dllexport) SendMessage(char* recepientId, MessageContent* msgData) {
		return SendMessage(recepientId, msgData);
	}*/
	void _declspec(dllexport) SendMessage(char* recepientId, char* msg, int msg_len) {
		std::vector<unsigned char>data(msg, msg + msg_len);
		Data datad =(Data) data;
		MessageContent msgData = MessageContent();
		msgData.data = datad;
		g_messenger->SendMessage(recepientId, msgData);
	}
	void _declspec(dllexport) SendMessageSeen(char* userId, char* msgId) {
		g_messenger->SendMessageSeen(userId,msgId);
	}


	void __declspec(dllexport) RegisterObserver(StatusChanged statusChanged, MessageReceived messageReceived) {
		g_messagesObserver.statusChanged = statusChanged;
		g_messagesObserver.messageReceived = messageReceived;
		g_messenger->RegisterObserver(&g_messagesObserver);
	}
}
